using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    /// A blob of ink.
    /// Can change the color of stuff.
    /// </summary>
    public class InkBlob : GameObject
    {
        #region Constants

        private const float DECELLERATION_X = 0.5f;

        private const float MAX_SPEED_X = 20f;
        private const float MAX_SPEED_Y = 30f;

        private const float BOUNCE_FRICTION = 2f;
        private const float BOUNCE_SIZE_REDUCTION = 10f;

        private const string FALL = "Fall";
        private const int FALL_START_FRAME = 0;
        private const string EXPLODE = "Explode";
        private const int EXPLODE_START_FRAME = 1;

        private const int FRAME_DURATION = 3;

        #endregion

        #region Members

        private bool shouldBounce;
        #endregion

        public InkBlob(Vector2 position, Vector2 initialVelocity, Color color, Vector2 size, bool shouldBounce=false, bool isWater=false) 
            : base(position, initialVelocity, new Vector2(MAX_SPEED_X, MAX_SPEED_Y), Vector2.Zero, new Vector2(DECELLERATION_X, 0),
            color, true, size, new List<AnimationSet>
                {
                    new AnimationSet(FALL, GetTexture(color, isWater), 1, 50, FRAME_DURATION, false, FALL_START_FRAME),
                    new AnimationSet(EXPLODE, GetTexture(color, isWater), 1, 50, FRAME_DURATION, false, EXPLODE_START_FRAME)
                }, FALL,VectorHelper.ToAngle(initialVelocity))
        {
            this.shouldBounce = shouldBounce;
        }

        public static Texture2D GetTexture(Color color, bool isWater)
        {
            if (isWater)
                return ResourceManager.GetTexture("Blob_Main_Water");
            if (color == Color.Black)
                return ResourceManager.GetTexture("Blob_Main_Black");

            return ResourceManager.GetTexture("Blob_Main");
        }

        public override void Update(Room room, GameTime gameTime)
        {
            // keep moving along
            Move(room, Vector2.Zero);
            if (curAnimation.IsCalled(EXPLODE) && curAnimation.IsDonePlaying())
                ChangeAnimation(FALL);
            base.Update(room, gameTime);
        }

        public override void CollideWithObject(GameObject obj, Room room, BBox collision)
        {
            base.CollideWithObject(obj, room, collision);
            // ink blobs should smash into each other, but avoid awkward behavior with ink blobs
            // coming out of the same generator
            if (obj is InkBlob && !(obj is WaterBlob) && (obj.Color != Color))
                room.Add(new InkBlob(Position, Vector2.Lerp(Velocity, obj.Velocity, 0.5f), Color.Combine(obj.Color),
                    Vector2.Lerp(Size, obj.Size, 0.5f), shouldBounce));
            room.Remove(this);
        }

        public override void CollideWithWall(Room room)
        {
            ChangeAnimation(EXPLODE);
            Vector2 splatPosition = Position.ShoveToSide(size, velocity);
            if (!(this is WaterBlob))
                // room.Splat(splatPosition, size, color, velocity);
            base.CollideWithWall(room);
            if (shouldBounce)
            {
                velocity *= -1;
                velocity = velocity.PushBack(BOUNCE_FRICTION * Vector2.One);
                size = size.PushBack(BOUNCE_SIZE_REDUCTION * Vector2.One);
                if (size == Vector2.Zero)
                    room.Remove(this);
            } else
                room.Remove(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frameRect = curAnimation.GetFrameRect();
            spriteBatch.Draw(curAnimation.GetTexture(), new Vector2(Center.X, Center.Y + 15), frameRect, Color == Color.Black ? Color.White : color, (Velocity * -Vector2.One).ToAngle(), 
                new Vector2(frameRect.Width/2, frameRect.Height/2), new Vector2(size.X/frameRect.Width, size.Y/frameRect.Height),
                SpriteEffects.None, 0);
        }
    }
}
