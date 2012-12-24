using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiledLib;

namespace Trauma.Helpers
{
    public static class MapHelper
    {
        public static int WidthInPixels(this Map map)
        {
            return map.Width * map.TileWidth;
        }
        
        public static int HeightInPixels(this Map map)
        {
            return map.Height * map.TileHeight;
        }
    }
}
