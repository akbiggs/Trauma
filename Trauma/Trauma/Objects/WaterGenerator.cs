using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Trauma.Objects
{
    public class WaterGenerator : InkGenerator
    {
        public WaterGenerator(Vector2 position, Vector2 blobDirection, int interval, float speed) : 
            base(position, blobDirection, Color.LightBlue, interval, speed)
        {
        }

        protected override InkBlob Generate()
        {
            InkBlob baseBlob = base.Generate();
            return new WaterBlob(baseBlob.Position, baseBlob.Velocity, baseBlob.Size);
        }
    }
}
