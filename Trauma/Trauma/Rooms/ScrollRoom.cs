using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;

namespace Trauma.Rooms
{
    /// <summary>
    /// A room that has the camera scrolling.
    /// Only room where the player can "lose", sending them back to the beginning.
    /// </summary>
    class ScrollRoom : Room
    {
        /// <summary>
        /// Create a new scroll room from the given map.
        /// </summary>
        /// <param name="map">The map of the room.</param>
        public ScrollRoom(Color color, Map map, GraphicsDevice device) : base(RoomType.Bargain, color, map, device)
        {
        }
    }
}
