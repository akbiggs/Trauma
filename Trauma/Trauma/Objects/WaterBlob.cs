using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Trauma.Objects
{
    public class WaterBlob : InkBlob
    {
        public WaterBlob(Vector2 position, Vector2 speed, Vector2 size) : 
            base(position, speed, Color.LightBlue, size, false, true)
        {
        }
    }
}
