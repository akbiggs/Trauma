using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Trauma.Engine;

namespace Trauma.Objects
{
    public class WaveGenerator : GameObject
    {
        const int INTERVAL = 100;
        const float WAVE_SPEED_X = 6f;

        int intervalTimer = (int)INTERVAL;
        bool facingLeft;
        bool isWater;
        Vector2 waveSize;

        public WaveGenerator(Vector2 position, Vector2 waveSize, bool facingLeft, Color color, bool isWater) :
            base(position, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero,
            color, true, Vector2.Zero, ResourceManager.GetTexture("Misc_Pixel"), 0)
        {
            this.facingLeft = facingLeft;
            this.isWater = isWater;
            this.waveSize = waveSize;
        }

        public override void Update(Rooms.Room room, GameTime gameTime)
        {

            if (intervalTimer == INTERVAL)
            {
                intervalTimer = 0;
                Generate(room);
            }

            intervalTimer++;
            base.Update(room, gameTime);
        }

        private void Generate(Rooms.Room room)
        {
            room.Add(new Wave(Position, waveSize,
                facingLeft ? new Vector2(-WAVE_SPEED_X, 0f) : new Vector2(WAVE_SPEED_X, 0f),
                Color, isWater));
        }
    }
}
