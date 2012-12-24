using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;

namespace Trauma.Objects
{
    /// <summary>
    /// Generates ink particles with passion.
    /// </summary>
    public class InkGenerator : GameObject
    {
        public InkGenerator(Vector2 position, Vector2 initialVelocity, Vector2 maxSpeed, Vector2 acceleration, Vector2 deceleration, Color color, bool colorable, Vector2 size, List<AnimationSet> animations, string startAnimationName, float rotation) : base(position, initialVelocity, maxSpeed, acceleration, deceleration, color, colorable, size, animations, startAnimationName, rotation)
        {
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
