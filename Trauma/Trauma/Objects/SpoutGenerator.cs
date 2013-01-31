using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;

namespace Trauma.Objects
{
    public class SpoutGenerator : GameObject
    {
        const int INTERVAL = 60;

        Vector2 direction;
        int intervalTimer = INTERVAL;
        bool isWater;

        public SpoutGenerator(Vector2 position, Vector2 direction, Color color, bool isWater)
            : base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, color,
            true, Vector2.Zero, ResourceManager.GetTexture("Misc_Pixel"), 0)
        {
            this.direction = direction;
            this.isWater = isWater;
        }

        public override void Update(Rooms.Room room, GameTime gameTime)
        {
            intervalTimer++;

            if (intervalTimer >= INTERVAL)
            {
                intervalTimer = 0;
                Generate(room);
            }

            base.Update(room, gameTime);
        }

        private void Generate(Rooms.Room room)
        {
            Vector2 spoutPos = Center;
            if (direction.X < 0)
                spoutPos = new Vector2(Position.X - 175, Center.Y);
            if (direction.X > 0)
                spoutPos = new Vector2(Position.X + room.Tilesize.X + Spout.SIZE.Y*(3f/4f), Center.Y);
            room.Add(new Spout(spoutPos, color, direction, isWater));
        }
    }
}
