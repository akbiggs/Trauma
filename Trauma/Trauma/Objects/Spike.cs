using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Objects;
using Trauma.Engine;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Helpers;
using System.Diagnostics;

namespace Trauma.Rooms
{
    public class Spike : GameObject
    {
        public Spike(Vector2 position, Vector2 size, Color color, Vector2 direction)
            : base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, color, true, size,
            ResourceManager.GetTexture("Misc_Spikes"), direction.ToAngle())
        {
            // facing up
            if (direction.X == 0 && direction.Y < 0) 
            {
                Facing = Direction.Up;
                this.Position = new Vector2(position.X, position.Y);
                this.Box.Position = new Vector2(position.X + size.X / 2, position.Y);
            }

            // facing down
            if (direction.X == 0 && direction.Y > 0)
            {
                Facing = Direction.Down;
                this.Position = new Vector2(position.X + size.X, position.Y + size.Y);
                this.box.Position = new Vector2(position.X, position.Y);
            }

            // facing left
            if (direction.X < 0 && direction.Y == 0)
            {
                Facing = Direction.Left;
                this.Position = new Vector2(position.X, position.Y + size.Y);
                this.box.Position = new Vector2(position.X, position.Y);
            }

            // facing right
            if (direction.X > 0 && direction.Y == 0)
            {
                Facing = Direction.Right;
                this.Position = new Vector2(position.X + size.X / 2, position.Y);
                this.box.Position = new Vector2(position.X, position.Y);
            }

            Debug.Assert(Facing != Direction.None, "Didn't handle a spike direction for facing.");
        }

        public override void Update(Room room, GameTime gameTime)
        {
            base.Update(room, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }

    public enum Direction
    {
        Up, Down, Left, Right, None
    }
}
