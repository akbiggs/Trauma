using System;
using System.Collections.Generic;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    ///     The main character, controlled by the player of the game.
    /// </summary>
    public class Player : GameObject
    {
        #region Constants

        private const int LEFT = -1;
        private const int RIGHT = 1;

        private const string SPLATTER_TAG = "_Splatter";

        private const float SPLATTER_IGNORE = 10f;
        private const float SPLAT_INTERVAL = 5f;
        private const float NEGLIGIBLE_SPLATTER_VELOCITY = 0.1f;

        private const float JUMP_SPLATTER_SIZE = 40f;
        private const float SLIDE_SPLATTER_SIZE = 30f;

        private const int SPLATTER_OFFSET_X = 17;
        private const int SPLATTER_OFFSET_Y = 4;
        private const float SPLATTER_RESIZE_X = 1.82f;
        private const float SPLATTER_RESIZE_Y = 1.36f;

        private const float MAX_SPEED_X = 6.5f;
        private const float MAX_SPEED_Y = 30f;

        private const float JUMP_SPEED = 20f;
        // minimum speed the player needs to collide with the ground to play landing animation
        private const float MIN_LAND_SPEED = 20f;
        // minimum speed the player needs to attain before we consider them as off the ground,
        // for example when they fall off a platform without jumping
        private const float MIN_FALL_SPEED = 4f;
        private const float WALL_SLIDE_FACTOR = 4f;

        private const float ACCELERATION_X = 1.0f;
        private const float ACCELERATION_Y = 1.5f;

        private const float DECCELERATION_X = 3.0f;
        private const float DECCLERATION_Y = 0f;

        private const int IDLE_FRAME_WIDTH = 100;
        private const int WALK_FRAME_WIDTH = 100;
        private const int JUMP_FRAME_WIDTH = 100;
        private const int SLIDE_FRAME_WIDTH = 100;
        private const int LAND_FRAME_WIDTH = 100;

        private const int WIDTH = 64;
        private const int HEIGHT = 64;

        private const String IDLE = "Idle";
        private const int IDLE_START_FRAME = 0;
        private const int IDLE_NUM_FRAMES = 1;

        private const String WALK = "Walk";
        private const int WALK_START_FRAME = 4;
        private const int WALK_NUM_FRAMES = 8;

        private const String JUMP = "Jump";
        private const int JUMP_START_FRAME = 1;
        private const int JUMP_NUM_FRAMES = 1;

        private const String SLIDE = "Slide";
        private const int SLIDE_START_FRAME = 3;
        private const int SLIDE_NUM_FRAMES = 1;

        private const String LAND = "Land";
        private const int LAND_START_FRAME = 2;
        private const int LAND_NUM_FRAMES = 1;

        private const int FRAME_DURATION = 3;
        // how many update cycles the player should be forced to stay landing for
        private const int LAND_DURATION = 20;

        #endregion

        #region Members

        private readonly List<Keys> jumpKeys;
        private readonly List<Keys> leftKeys;
        private readonly List<Keys> rightKeys;
        private bool canJump = true;
        private bool onGround;
        private int facing = RIGHT;
        private bool Landing
        {
            get { return curAnimation.IsCalled(LAND) && !curAnimation.IsDonePlaying(); }
            set { if (value) ChangeAnimation(LAND); }
        }

        private AnimationSet splatterAnimation;

        private bool canSplat = true;
        private Timer splatTimer;

        protected override AnimationSet curAnimation
        {
            set
            {
                base.curAnimation = value;
                //if ((splatterAnimation = animations.Find(anim => anim.IsCalled(curAnimation.Name + SPLATTER_TAG))) ==
                //    null)
                //{
                //    splatterAnimation = new AnimationSet(curAnimation.Name + "_" + SPLATTER_TAG,
                //                                         ResourceManager.GetTexture("Player" + "_" + curAnimation.Name +
                //                                                                    "_" + SPLATTER_TAG),
                //                                         curAnimation.NumFrames,
                //                                         373, curAnimation.FrameDuration);
                //    animations.Add(splatterAnimation);
                //}
            }
        }

        #endregion

        public Player(Vector2 pos) :
            base(pos, Vector2.Zero, new Vector2(MAX_SPEED_X, MAX_SPEED_Y),
                 new Vector2(ACCELERATION_X, ACCELERATION_Y), new Vector2(DECCELERATION_X, DECCLERATION_Y),
                 Color.Red, true, new Vector2(WIDTH, HEIGHT),
                 new List<AnimationSet>
                     {
                         new AnimationSet(IDLE, ResourceManager.GetTexture("Player_Main"), IDLE_NUM_FRAMES, 
                             IDLE_FRAME_WIDTH, FRAME_DURATION),
                         new AnimationSet(IDLE + SPLATTER_TAG, ResourceManager.GetTexture("Player_Main_Splatter"), IDLE_NUM_FRAMES,
                             IDLE_FRAME_WIDTH, FRAME_DURATION),
                         new AnimationSet(WALK, ResourceManager.GetTexture("Player_Main"), WALK_NUM_FRAMES, 
                             WALK_FRAME_WIDTH, FRAME_DURATION, true, WALK_START_FRAME),
                         new AnimationSet(WALK + SPLATTER_TAG, ResourceManager.GetTexture("Player_Main_Splatter"), WALK_NUM_FRAMES,
                             WALK_FRAME_WIDTH, FRAME_DURATION, true, WALK_START_FRAME),
                         new AnimationSet(JUMP, ResourceManager.GetTexture("Player_Main"), JUMP_NUM_FRAMES,
                             JUMP_FRAME_WIDTH, FRAME_DURATION, false, JUMP_START_FRAME),
                         new AnimationSet(JUMP + SPLATTER_TAG, ResourceManager.GetTexture("Player_Main_Splatter"), JUMP_NUM_FRAMES,
                             JUMP_FRAME_WIDTH, FRAME_DURATION, false, JUMP_START_FRAME),
                         new AnimationSet(SLIDE, ResourceManager.GetTexture("Player_Main"), SLIDE_NUM_FRAMES, 
                             SLIDE_FRAME_WIDTH, FRAME_DURATION, true, SLIDE_START_FRAME),
                         new AnimationSet(SLIDE + SPLATTER_TAG, ResourceManager.GetTexture("Player_Main_Splatter"), SLIDE_NUM_FRAMES,
                             SLIDE_FRAME_WIDTH, FRAME_DURATION, true, SLIDE_START_FRAME),
                         new AnimationSet(LAND, ResourceManager.GetTexture("Player_Main"), LAND_NUM_FRAMES,
                             LAND_FRAME_WIDTH, LAND_DURATION, false, LAND_START_FRAME),
                         new AnimationSet(LAND + SPLATTER_TAG, ResourceManager.GetTexture("Player_Main_Splatter"), LAND_NUM_FRAMES,
                             LAND_FRAME_WIDTH, LAND_DURATION, false, LAND_START_FRAME)
                     },
                 JUMP, 0)
        {
            jumpKeys = new List<Keys>{Keys.W, Keys.Up};
            leftKeys = new List<Keys> {Keys.A, Keys.Left};
            rightKeys = new List<Keys> {Keys.D, Keys.Right};

            splatTimer = new Timer {AutoReset = true, Interval = SPLAT_INTERVAL};
            splatTimer.Elapsed += (sender, args) => canSplat = true;
            splatTimer.Start();

            splatterAnimation = animations.Find((anim) => anim.IsCalled(curAnimation.Name + SPLATTER_TAG));
        }

        public override void Update(Room room, GameTime gameTime)
        {
            base.Update(room, gameTime);
            splatterAnimation.Update();

            if (canJump && jumpKeys.Any(Input.IsKeyDown))
                DoJump(room);

            // wait until we've recovered from the recoil of landing before moving again
            if (!Landing)
            {
                if (leftKeys.Any(Input.IsKeyDown))
                    Move(room, new Vector2(-1, 0));
                else if (rightKeys.Any(Input.IsKeyDown))
                    Move(room, new Vector2(1, 0));
                else
                    Move(room, Vector2.Zero);
            }
            // we still need to call Move at least once per update for collisions etc.
            else
                Move(room, new Vector2(0, 0));

            if (Math.Abs(Velocity.Y) >= MIN_FALL_SPEED)
                onGround = false;

            // wait until the landing animation is done playing before changing it
            if (onGround && !Landing)
            {
                Landing = false;
                if (!curAnimation.IsCalled(WALK)) ChangeAnimation(IDLE);
            } else if (!hasCollidedWithWall)
                ChangeAnimation(JUMP);
            
            if (onGround && Math.Abs(Velocity.X) > 0)
                ChangeAnimation(WALK);
        }

        internal override void Move(Room room, Vector2 direction)
        {
            if (direction.X == 0 && curAnimation.IsCalled(WALK))
                ChangeAnimation(IDLE);
            
            if (direction.X < 0)
                facing = LEFT;
            else if (direction.X > 0)
                facing = RIGHT;

            base.Move(room, direction);
        }

        public override void CollideWithWall(Room room)
        {
            base.CollideWithWall(room);
            if (Velocity.Y > 0 && !onGround)
            {
                Velocity = Velocity.PushBack(new Vector2(0, Velocity.Y/WALL_SLIDE_FACTOR));
                if (!curAnimation.IsCalled(LAND))
                    ChangeAnimation(SLIDE);
                canJump = true;

                float splatX = Velocity.X > 0 ? Position.X + size.X : Position.X;
                float splatY = Position.Y + size.Y;
                room.Splat(new Vector2(splatX, splatY), Vector2.One * SLIDE_SPLATTER_SIZE, 
                    color, Velocity);
            }

            // splat if the impact is too big to ignore.
            if (Velocity.LengthSquared() > Math.Pow(SPLATTER_IGNORE, 2))
            {
                Vector2 splatPosition = Position.ShoveToSide(size, Velocity);

                if (canSplat)
                {
                    room.Splat(splatPosition, size, color,
                               new Vector2(Math.Abs(Velocity.X) > NEGLIGIBLE_SPLATTER_VELOCITY ? Velocity.X : 0,
                                           Math.Abs(Velocity.Y) > NEGLIGIBLE_SPLATTER_VELOCITY ? Velocity.Y : 0));
                    canSplat = false;
                    splatTimer.Start();
                }
            }
        }

        public override void CollideWithObject(GameObject obj, Room room, BBox collision)
        {
            if (obj is InkBlob)
                color = color.Combine(obj.Color);
            base.CollideWithObject(obj, room, collision);
        }

        public override void CollideWithGround(Room room)
        {
            if (!onGround && Velocity.Y >= MIN_LAND_SPEED)
            {
                Velocity.X = 0;
                Land();
            }
            Velocity.Y = 0;
            onGround = true;
            canJump = true;
            base.CollideWithGround(room);
        }

        /// <summary>
        ///     Jumps the player.
        /// </summary>
        private void DoJump(Room room)
        {
            ChangeAnimation(JUMP);
            Velocity.Y = -JUMP_SPEED;
            canJump = false;
            room.Splat(Position, Vector2.One * JUMP_SPLATTER_SIZE, color, Velocity);
        }

        /// <summary>
        ///     Lands the player.
        /// </summary>
        internal void Land()
        {
            ChangeAnimation(LAND);
            Landing = true;
        }

        protected override void ChangeAnimation(string name)
        {
            AnimationSet oldAnimation = curAnimation;
            base.ChangeAnimation(name);
            // only reset the splatter animation if the animation changed
            if (oldAnimation != curAnimation)
            {
                splatterAnimation = animations.Find((anim) => anim.IsCalled(name + SPLATTER_TAG));
                splatterAnimation.Reset();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation.GetTexture(),
                             new Rectangle((int) Position.X, (int) Position.Y, (int) size.X, (int) size.Y),
                             curAnimation.GetFrameRect(), Color.White, 
                             0,
                             Vector2.Zero,
                             ShouldBeFlipped() ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 
                             0);

            if (color != Color.Black)
                spriteBatch.Draw(splatterAnimation.GetTexture(),
                                 new Rectangle((int) Position.X, (int) Position.Y, (int) size.X, (int) size.Y),
                                 curAnimation.GetFrameRect(), GetDrawColor(),
                                 0,
                                 Vector2.Zero,
                                 ShouldBeFlipped() ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                 0);

            //spriteBatch.Draw(splatterAnimation.GetTexture(),
            //                 new Rectangle((int) position.X - SPLATTER_OFFSET_X, (int) position.Y - SPLATTER_OFFSET_Y,
            //                               (int) (size.X*SPLATTER_RESIZE_X), (in  t) (size.Y*SPLATTER_RESIZE_Y)),
            //                 splatterAnimation.GetFrameRect(), color);
        }

        private Color GetDrawColor()
        {
            if (color == Color.Black)
                return Color.White;

            return color;
        }

        private bool ShouldBeRotated()
        {
            return curAnimation.IsCalled(JUMP);
        }

        private bool ShouldBeFlipped()
        {
            if (curAnimation.IsCalled(SLIDE))
                return facing == RIGHT;
            return facing == LEFT;
        }
    }
}