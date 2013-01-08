using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
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

        private const float DECELLERATION_X = 2f;

        private const float MAX_SPEED_X = 20f;
        private const float MAX_SPEED_Y = 30f;

        private const float BOUNCE_FRICTION = 2f;
        private const float BOUNCE_SIZE_REDUCTION = 10f;
        #endregion

        #region Members

        private bool shouldBounce;
        #endregion

        public InkBlob(Vector2 position, Vector2 initialVelocity, Color color, Vector2 size, bool shouldBounce=true) 
            : base(position, initialVelocity, new Vector2(MAX_SPEED_X, MAX_SPEED_Y), Vector2.Zero, new Vector2(DECELLERATION_X, 0),
            color, true, size, ResourceManager.GetTexture("Misc_Pixel"), VectorHelper.ToAngle(initialVelocity))
        {
            this.shouldBounce = shouldBounce;
        }

        public override void Update(Room room, GameTime gameTime)
        {
            // keep moving along
            Move(room, Vector2.Zero);
            base.Update(room, gameTime);
        }

        public override void CollideWithObject(GameObject obj, Room room, BBox collision)
        {
            base.CollideWithObject(obj, room, collision);
            room.Remove(this);
        }

        public override void CollideWithWall(Room room)
        {
            Vector2 splatPosition = Position.ShoveToSide(size, Velocity);
            room.Splat(splatPosition, size, color, Velocity);
            base.CollideWithWall(room);
            if (shouldBounce)
            {
                Velocity *= -1;
                Velocity = Velocity.PushBack(BOUNCE_FRICTION * Vector2.One);
                size = size.PushBack(BOUNCE_SIZE_REDUCTION * Vector2.One);
                if (size == Vector2.Zero)
                    room.Remove(this);
            } else
                room.Remove(this);
        }
    }
}
