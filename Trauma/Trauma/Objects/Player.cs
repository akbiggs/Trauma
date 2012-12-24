using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    /// The main character, controlled by the player of the game.
    /// </summary>
    public class Player : GameObject
    {
        #region Constants

        private const float MAX_SPEED_X = 8f;
        private const float MAX_SPEED_Y = 20f;
        private const float JUMP_SPEED = 17f;
        private const float WALL_SLIDE_FACTOR = 4f;

        private const float ACCELERATION_X = 1.0f;
        private const float ACCELERATION_Y = 1.5f;

        private const float DECCELERATION_X = 3.0f;
        private const float DECCLERATION_Y = 0f;

        private const int WIDTH = 24;
        private const int HEIGHT = 24;

        private const int FRAME_DURATION = 1;

        #endregion

        #region Members

        private bool canJump = true;
        private Keys Jump, Left, Right; 
        
        #endregion

        public Player(Vector2 pos) : 
            base(pos, Vector2.Zero, new Vector2(MAX_SPEED_X, MAX_SPEED_Y), 
            new Vector2(ACCELERATION_X, ACCELERATION_Y), new Vector2(DECCELERATION_X, DECCLERATION_Y),
            Color.Black, true, new Vector2(WIDTH, HEIGHT),
            new List<AnimationSet>
                {
                    // TODO: Replace this with actual animations.
                    new AnimationSet("idle", ResourceManager.GetTexture("Misc_Pixel"), 1, WIDTH, FRAME_DURATION)
                }, 
            "idle", 0)
        {
            Jump = Keys.W;
            Left = Keys.A;
            Right = Keys.D;
        }

        public override void Update(Room room, GameTime gameTime)
        {
            if (canJump && Input.KeyboardTapped(Jump))
                DoJump();

            if (Input.IsKeyDown(Left))
                Move(room, new Vector2(-1, 0));
            else if (Input.IsKeyDown(Right))
                Move(room, new Vector2(1, 0));
            else
                Move(room, new Vector2(0, 0));
            
            base.Update(room, gameTime);
        }

        public override void CollideWithWall(Room room)
        {
            base.CollideWithWall(room);
            if (Velocity.Y > 0)
                Velocity = Velocity.PushBack(new Vector2(0, Velocity.Y / WALL_SLIDE_FACTOR));
            room.Splat(position, size, color, Velocity);
        }

        public override void CollideWithGround(Room room)
        {
            Land();
            base.CollideWithGround(room);
        }

        /// <summary>
        /// Jumps the player.
        /// </summary>
        private void DoJump()
        {
            Velocity.Y = -JUMP_SPEED;
            canJump = false;
        }


        /// <summary>
        /// Lands the player.
        /// </summary>
        internal void Land()
        {
            // TODO: If we get a landing animation, don't forget to play it here.
            canJump = true;
        }
    }
}