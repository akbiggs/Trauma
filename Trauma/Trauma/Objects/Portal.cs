using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Trauma.Rooms;

namespace Trauma.Objects
{
    /// <summary>
    /// The exit portal to any room.
    /// Allows the player if they're the same color as the portal,
    /// otherwise rejects.
    /// </summary>
    public class Portal : GameObject
    {
        public Portal(Vector2 position, Vector2 initialVelocity, Vector2 maxSpeed, Vector2 acceleration, Vector2 deceleration, Color color, bool colorable, Vector2 size, List<AnimationSet> animations, string startAnimationName, float rotation) : base(position, initialVelocity, maxSpeed, acceleration, deceleration, color, colorable, size, animations, startAnimationName, rotation)
        {
        }

        public override void Update(Room room, GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
