using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Trauma.Objects
{
    public class Barrier : GameObject
    {
        public Barrier(Vector2 position, Vector2 size, Vector2 direction, Color color)
            : base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero,
            color, true, size, new List<AnimationSet>
            {
                new AnimationSet("Up", ResourceManager.GetTexture("Misc_Barrier"), 1, 50, 1, false),
                new AnimationSet("Right", ResourceManager.GetTexture("Misc_Barrier"), 1, 50, 1, false, 1),
                new AnimationSet("Down", ResourceManager.GetTexture("Misc_Barrier"), 1, 50, 1, false, 2),
                new AnimationSet("Left", ResourceManager.GetTexture("Misc_Barrier"), 1, 50, 1, false, 3)
            }, GetStartFrame(direction), 0)
        {

        }

        public static String GetStartFrame(Vector2 direction)
        {
            direction.Normalize();
            if (direction.X < 0)
                return "Left";
            if (direction.X > 0)
                return "Right";
            if (direction.Y < 0)
                return "Up";
            if (direction.Y > 0)
                return "Down";

            throw new InvalidOperationException("Invalid barrier direction specified.");
        }
    }
}
