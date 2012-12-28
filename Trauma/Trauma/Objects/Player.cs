using System.Collections.Generic;
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

        private const string SPLATTER_TAG = "Splatter";

        private const int SPLATTER_OFFSET_X = 17;
        private const int SPLATTER_OFFSET_Y = 4;
        private const float SPLATTER_RESIZE_X = 1.82f;
        private const float SPLATTER_RESIZE_Y = 1.36f;

        private const float MAX_SPEED_X = 8f;
        private const float MAX_SPEED_Y = 20f;
        private const float JUMP_SPEED = 20f;
        private const float WALL_SLIDE_FACTOR = 4f;

        private const float ACCELERATION_X = 1.0f;
        private const float ACCELERATION_Y = 1.5f;

        private const float DECCELERATION_X = 3.0f;
        private const float DECCLERATION_Y = 0f;

        private const int FRAME_WIDTH = 200;

        private const int WIDTH = 48;
        private const int HEIGHT = 64;

        private const int FRAME_DURATION = 1;

        #endregion

        #region Members

        private readonly Keys jump;
        private readonly Keys left;
        private readonly Keys right;
        private bool canJump = true;
        private AnimationSet splatterAnimation;

        protected override AnimationSet curAnimation
        {
            set
            {
                base.curAnimation = value;
                if ((splatterAnimation = animations.Find(anim => anim.IsCalled(curAnimation.Name + SPLATTER_TAG))) ==
                    null)
                {
                    splatterAnimation = new AnimationSet(curAnimation.Name + "_" + SPLATTER_TAG,
                                                         ResourceManager.GetTexture("Player" + "_" + curAnimation.Name +
                                                                                    "_" + SPLATTER_TAG),
                                                         curAnimation.NumFrames,
                                                         373, curAnimation.FrameDuration);
                    animations.Add(splatterAnimation);
                }
            }
        }

        #endregion

        public Player(Vector2 pos) :
            base(pos, Vector2.Zero, new Vector2(MAX_SPEED_X, MAX_SPEED_Y),
                 new Vector2(ACCELERATION_X, ACCELERATION_Y), new Vector2(DECCELERATION_X, DECCLERATION_Y),
                 Color.Red, true, new Vector2(WIDTH, HEIGHT),
                 new List<AnimationSet>
                     {
                         // TODO: Replace this with actual animations.
                         new AnimationSet("Idle", ResourceManager.GetTexture("Player_Idle"), 1, FRAME_WIDTH,
                                          FRAME_DURATION)
                     },
                 "Idle", 0)
        {
            jump = Keys.W;
            left = Keys.A;
            right = Keys.D;
        }

        public override void Update(Room room, GameTime gameTime)
        {
            if (canJump && Input.KeyboardTapped(jump))
                DoJump();

            if (Input.IsKeyDown(left))
                Move(room, new Vector2(-1, 0));
            else if (Input.IsKeyDown(right))
                Move(room, new Vector2(1, 0));
            else
                Move(room, new Vector2(0, 0));

            base.Update(room, gameTime);
        }

        public override void CollideWithWall(Room room)
        {
            base.CollideWithWall(room);
            if (Velocity.Y > 0)
            {
                Velocity = Velocity.PushBack(new Vector2(0, Velocity.Y/WALL_SLIDE_FACTOR));
                // TODO: Maybe add in some kind of wall-sliding animation here?
                canJump = true;
            }
            room.Splat(position, size, color, Velocity);
        }

        public override void CollideWithGround(Room room)
        {
            Land();
            base.CollideWithGround(room);
        }

        /// <summary>
        ///     Jumps the player.
        /// </summary>
        private void DoJump()
        {
            Velocity.Y = -JUMP_SPEED;
            canJump = false;
        }


        /// <summary>
        ///     Lands the player.
        /// </summary>
        internal void Land()
        {
            // TODO: If we get a landing animation, don't forget to play it here.
            canJump = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curAnimation.GetTexture(),
                             new Rectangle((int) position.X, (int) position.Y, (int) size.X, (int) size.Y),
                             curAnimation.GetFrameRect(), Color.White);

            spriteBatch.Draw(splatterAnimation.GetTexture(),
                             new Rectangle((int) position.X - SPLATTER_OFFSET_X, (int) position.Y - SPLATTER_OFFSET_Y,
                                           (int) (size.X*SPLATTER_RESIZE_X), (int) (size.Y*SPLATTER_RESIZE_Y)),
                             splatterAnimation.GetFrameRect(), color);
        }
    }
}