﻿using System;
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
        private const float SPLATTER_RESIZE_FACTOR = 0.6f;

        private const float SPLATTER_IGNORE = 10f;
        private const float SPLAT_INTERVAL = 5f;
        private const float NEGLIGIBLE_SPLATTER_VELOCITY = 0.1f;

        private const float JUMP_SPLATTER_SIZE = 20f;
        private const float SLIDE_SPLATTER_SIZE = 30f;
        private const float STEP_SPLATTER_SIZE = 0.6f;

        private const float MAX_SPEED_X = 10f;
        private const float MAX_JUMP_SPEED_X = MAX_SPEED_X - 0.5f;
        private const float MAX_SPEED_Y = 40f;
        private const float MAX_JUMP_SPEED_Y = 20f;
        private const float MAX_RED_JUMP_SPEED_Y = 40f;

        private const float JUMP_SPEED = 16.75f;
        private const float RED_JUMP_SPEED = 26f;
        // minimum speed the player needs to collide with the ground to play landing animation
        private const float MIN_LAND_SPEED = 4f;
        // minimum speed the player needs to attain before we consider them as off the ground,
        // for example when they fall off a platform without jumping
        private const float MIN_FALL_SPEED = 2f;
        private const float MAX_BLUE_FALL_SPEED = 4f;
        private const float WALL_SLIDE_FACTOR = 4f;
        private const float WALL_JUMP_PUSH = MAX_JUMP_SPEED_X - 2;

        private const int NUM_DEATH_BLOBS = 10;
        private const int DEATH_BLOB_SPEED = 100;
        private const float DEATH_BLOB_SIZE = 20;

        private const float ACCELERATION_X = 1.2f;
        private const float ACCELERATION_Y = 1.5f;

        private const float DECCELERATION_X = 3.0f;
        private const float DECCLERATION_Y = 0f;

        private const int IDLE_FRAME_WIDTH = 100;
        private const int WALK_FRAME_WIDTH = 100;
        private const int JUMP_FRAME_WIDTH = 100;
        private const int SLIDE_FRAME_WIDTH = 100;
        private const int LAND_FRAME_WIDTH = 100;

        public const int DEATH_TIME = 30;

        private const int WIDTH = 130;
        private const int HEIGHT = 130;

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

        private const String DIE = "Die";
        private const int DIE_START_FRAME = 12;
        private const int DIE_NUM_FRAMES = 6;

        private const int FRAME_DURATION = 8;
        
        // how many frames the player takes a step 
        private const int FRAMES_PER_STEP = 4;
        private const int STEP_FRAME_OFFSET = 1;

        // how many update cycles the player should be forced to stay landing for
        private const int LAND_DURATION = 20;

        #endregion

        #region Members

        private readonly List<Keys> jumpKeys;
        private readonly List<Keys> leftKeys;
        private readonly List<Keys> rightKeys;
        private bool canJump = true;
        private bool onGround;
        private bool dying = false;
        public bool IsAlive
        {
            get { return !dying; }
        }
        private int deathTimer = 0;
        private int facing = RIGHT;
        private bool Landing
        {
            get
            {
                return false;
                //return curAnimation.IsCalled(LAND) && !curAnimation.IsDonePlaying(); 
            }
            set { if (value) ChangeAnimation(LAND); }
        }

        private AnimationSet splatterAnimation;

        private bool canSplat = true;
        private Timer splatTimer;

        public int GravitySign
        {
            get { return UnderReverseGravity() ? -1 : 1; }
        }

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
            base(pos, Vector2.Zero, new Vector2(MAX_JUMP_SPEED_X, MAX_JUMP_SPEED_Y),
                 new Vector2(ACCELERATION_X, ACCELERATION_Y), new Vector2(DECCELERATION_X, DECCLERATION_Y),
                 Color.Black, true, new Vector2(WIDTH, HEIGHT),
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
                             LAND_FRAME_WIDTH, LAND_DURATION, false, LAND_START_FRAME),
                         new AnimationSet(DIE, ResourceManager.GetTexture("Player_Main"), DIE_NUM_FRAMES, JUMP_FRAME_WIDTH, FRAME_DURATION,
                             false, DIE_START_FRAME)
                     },
                 JUMP, 0)
        {
            jumpKeys = new List<Keys>{Keys.W, Keys.Up, Keys.Space};
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
            if (!dying)
            {
                splatterAnimation.Update();

                if (Velocity.Y == 0)
                    maxSpeed.Y = MAX_SPEED_Y;
                if (canJump && jumpKeys.Any(Input.IsKeyDown))
                    DoJump(room);

                // allow player to shortcircuit their jump
                //if (Velocity.Y < -5 && !jumpKeys.Any(Input.IsKeyDown))
                //    velocity.Y = -5;

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

                if (Math.Abs(velocity.Y) >= MIN_FALL_SPEED * GravitySign)
                    onGround = false;

                // wait until the landing animation is done playing before changing it
                if (onGround && !Landing)
                {
                    Landing = false;
                    if (!curAnimation.IsCalled(WALK)) ChangeAnimation(IDLE);
                }
                else if (!hasCollidedWithWall)
                    ChangeAnimation(JUMP);

                if (onGround && Math.Abs(velocity.X) > 0)
                    ChangeAnimation(WALK);

                //if (color.Contains(Color.Blue) && (velocity.Y * GravitySign) > (MAX_BLUE_FALL_SPEED * GravitySign) && jumpKeys.Any(Input.IsKeyDown))
                //    velocity.Y = MAX_BLUE_FALL_SPEED * GravitySign;

                rotation = velocity.ToAngle();
            }
            else
            {
                deathTimer++;
                if (deathTimer == DEATH_TIME)
                {
                    room.Failed = true;
                    room.Finish();
                }
            }
        }

        protected override void ApplyGravity(Room room)
        {
            //if (Color.Contains(Color.Yellow))
            //    velocity.Y -= room.Gravity;
            //else
                base.ApplyGravity(room);
        }

        internal override void Move(Room room, Vector2 direction)
        {
            if (direction.X == 0 && curAnimation.IsCalled(WALK))
                ChangeAnimation(IDLE);

            if (direction.X < 0)
            {
                if (curAnimation.IsCalled(SLIDE))
                {
                    velocity.X = -2;
                    ChangeAnimation(JUMP);
                    Position = new Vector2(Position.X - size.X / 8, Position.Y);
                }
                facing = LEFT;
            }
            else if (direction.X > 0)
            {
                if (curAnimation.IsCalled(SLIDE))
                {
                    velocity.X = 2;
                    ChangeAnimation(JUMP);
                    Position = new Vector2(Position.X + size.X / 8, Position.Y);
                }
                facing = RIGHT;
            }

            if (TookStep && !(Color == Color.Black))
                room.Splat(facing == RIGHT ? new Vector2(box.Left, box.Bottom) : new Vector2(box.Right, box.Bottom), 
                    STEP_SPLATTER_SIZE * Vector2.One, color, room.Gravity * Vector2.One * 0.0001f);

            base.Move(room, direction);
        }

        public override void CollideWithWall(Room room)
        {
            base.CollideWithWall(room);

            if (Math.Abs(velocity.Y) > 0 && !onGround && !hasCollidedWithCeiling)
            {
                ChangeAnimation(SLIDE);
            }
            if (velocity.Y > 0 && !onGround)
            {
                velocity = velocity.PushBack(new Vector2(0, velocity.Y/WALL_SLIDE_FACTOR));
                canJump = true;

                float splatX = velocity.X > 0 ? Position.X + size.X : Position.X;
                float splatY = Position.Y + size.Y / 4;
                if (!hasCollidedWithCeiling && !(Color == Color.Black)) 
                    room.Splat(facing == LEFT ? new Vector2(splatX + 10, splatY) : new Vector2(splatX - 23, splatY), Vector2.One * 80f,
                    color, velocity);
            }

            // splat if the impact is too big to ignore.
            if (velocity.LengthSquared() > Math.Pow(SPLATTER_IGNORE, 2))
            {
                Vector2 splatPosition = Position.ShoveToSide(size, velocity);
                if (facing == LEFT)
                    splatPosition.Y += 30f;

                if (canSplat)
                {
                    if (!hasCollidedWithCeiling && !(Color == Color.Black)) 
                        room.Splat(splatPosition, size * STEP_SPLATTER_SIZE, color,
                               new Vector2(Math.Abs(velocity.X) > NEGLIGIBLE_SPLATTER_VELOCITY ? velocity.X : 0,
                                           Math.Abs(velocity.Y) > NEGLIGIBLE_SPLATTER_VELOCITY ? velocity.Y : 0));
                    canSplat = false;
                    splatTimer.Start();
                }
            }
        }

        public override void CollideWithObject(GameObject obj, Room room, BBox collision)
        {
            if (!dying && obj.IsCollidable) {
                // water is special, kills player
                if (obj is WaterBlob || (obj is Wave && obj.Color == Color.LightBlue) || 
                    (obj is Spout && obj.Color == Color.LightBlue && ShouldSpoutCollide(obj)))
                    
                    Die(room);
                else if (obj is InkBlob || obj is Wave || obj is Spout && ShouldSpoutCollide(obj))
                {
                    if (obj.Color == Color.Black)
                        color = Color.Black;
                    else
                        color = color.Combine(obj.Color);
                }
                if (obj is Portal)
                    if (color == obj.Color)
                    {
                        if (room.Portals.Count > 1 && obj != room.CorrectPortal)
                            room.Failed = true;
                        //room.Finish();
                    }
                if (obj as Spike != null)
                {
                    // only punish the player if they're moving towards the spikes, too unforgiving otherwise
                    if ((obj.Color == Color.White || obj.Color != Color) && (obj.Facing == Direction.Left && Velocity.X >= 0 ||
                        obj.Facing == Direction.Right && Velocity.X <= 0 ||
                        obj.Facing == Direction.Up && Velocity.Y >= 0 ||
                        obj.Facing == Direction.Down && Velocity.Y <= 0) && collision.Area >= box.Area / 7)
                        //&& Math.Max(MathHelper.Distance(Box.Center.X, obj.Box.Center.X), 
                        //    MathHelper.Distance(Box.Center.Y, obj.Box.Center.Y)) < 20f)
                        Die(room);
                }
            }

            base.CollideWithObject(obj, room, collision);
        }

        private bool ShouldSpoutCollide(GameObject obj)
        {
            return true;
        }

        public override void CollideWithGround(Room room)
        {
        //    if (Velocity.Y > 6f && !(Color == Color.Black))
        //        room.Splat(facing == LEFT ? new Vector2(Position.X, Position.Y + 160) : new Vector2(Position.X, Position.Y + 160), 
        //            Vector2.One * JUMP_SPLATTER_SIZE*3, color, velocity);

            maxSpeed = new Vector2(MAX_SPEED_X, MAX_SPEED_Y);
            velocity.Y = 0;
            onGround = true;
            canJump = true;
            base.CollideWithGround(room);
        }

        protected override void CollideWithCeiling(Room room)
        {
            if (UnderReverseGravity())
            {
                maxSpeed = new Vector2(MAX_SPEED_X, MAX_SPEED_Y);
                velocity.Y = 0;
                onGround = true;
                canJump = true;
            }
            base.CollideWithCeiling(room);
        }

        /// <summary>
        ///     Jumps the player.
        /// </summary>
        private void DoJump(Room room)
        {
            velocity.Y = -JUMP_SPEED;
            if (curAnimation.IsCalled(SLIDE))
                velocity.X = ((facing == RIGHT && velocity.X > 0) ? -WALL_JUMP_PUSH : WALL_JUMP_PUSH);

            maxSpeed = new Vector2(MAX_JUMP_SPEED_X, MAX_JUMP_SPEED_Y);

            ChangeAnimation(JUMP);
            canJump = false;
            if (!(Color == Color.Black)) 
                room.Splat(Center, Vector2.One * JUMP_SPLATTER_SIZE*2.5f, color, velocity);
        }

        /// <summary>
        ///     Lands the player.
        /// </summary>
        internal void Land()
        {
            ChangeAnimation(LAND);
            Landing = true;
        }

        public void Die(Room room)
        {
            room.Splat(Center, size * 8, color, Vector2.One);
            dying = true;
            ChangeAnimation(DIE);
        }

        protected override void ChangeAnimation(string name)
        {
            AnimationSet oldAnimation = curAnimation;
            base.ChangeAnimation(name);
            // only reset the splatter animation if the animation changed
            if (oldAnimation != curAnimation && !curAnimation.IsCalled(DIE))
            {
                splatterAnimation = animations.Find((anim) => anim.IsCalled(name + SPLATTER_TAG));
                splatterAnimation.Reset();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
            Rectangle frameRect = curAnimation.GetFrameRect();
            spriteBatch.Draw(curAnimation.GetTexture(),
                                Position + new Vector2(0, 8),
                                frameRect, Color.White,
                                UnderReverseGravity() ? (float)Math.PI : 0,
                                Vector2.Zero, new Vector2(size.X / frameRect.Width, size.Y / frameRect.Height),
                                ShouldBeFlipped() ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                0);
            if (!dying)
            {
                if (color != Color.Black && !curAnimation.IsCalled(DIE))
                    spriteBatch.Draw(splatterAnimation.GetTexture(),
                                     Position + new Vector2(0, 8),
                                     frameRect, GetDrawColor(),
                                     UnderReverseGravity() ? (float)Math.PI : 0,
                        //ShouldBeRotated() ? rotation : 0,
                                     Vector2.Zero, new Vector2(size.X / frameRect.Width, size.Y / frameRect.Height),
                                     ShouldBeFlipped() ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                                     0);

                //spriteBatch.Draw(splatterAnimation.GetTexture(),
                //                 new Rectangle((int) position.X - SPLATTER_OFFSET_X, (int) position.Y - SPLATTER_OFFSET_Y,
                //                               (int) (size.X*SPLATTER_RESIZE_X), (in  t) (size.Y*SPLATTER_RESIZE_Y)),
                //                 splatterAnimation.GetFrameRect(), color);
            }
        }

        private bool UnderReverseGravity()
        {
            return false;
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

        public bool TookStep
        {
            get { return curAnimation.CurFrameNumber%FRAMES_PER_STEP == STEP_FRAME_OFFSET; }
        }

        internal void Undo()
        {
            // TODO: Undo the player's color to the previous color.
        }
    }
}