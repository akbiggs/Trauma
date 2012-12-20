using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Trauma.Engine
{
    /// <summary>
    /// A bounding box for an object.
    /// Can be used for collision detection.
    /// </summary>
    public class BBox
    {
        private Rectangle box;

        public BBox(Rectangle rectangle)
        {
            box = rectangle;
        }
    }
}
