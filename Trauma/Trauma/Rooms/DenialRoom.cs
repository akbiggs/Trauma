using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using Trauma.Objects;

namespace Trauma.Rooms
{
    /// <summary>
    /// A room for the denial section of the game.
    /// May no longer feature special properties...stay tuned.
    /// </summary>
    public class DenialRoom : Room
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DenialRoom" /> class.
        /// </summary>
        /// <param name="map">The map this room is based off of.</param>
        public DenialRoom(Color color, Map map, GraphicsDevice device) : base(RoomType.Denial, color, map, device)
        {
            
        }
    }
}
