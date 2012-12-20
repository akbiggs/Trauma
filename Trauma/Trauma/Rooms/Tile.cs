using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;

namespace Trauma.Rooms
{
    /// <summary>
    /// A tile of the room.
    /// Can be solid, passable on the underside, or transparent.
    /// </summary>
    public class Tile
    {
        private const string TYPE_PROPERTY_NAME = "Type";
        private const string SOLID = "Solid";
        private const string TOP_SOLID = "TopSolid";
        private const string TRANSPARENT = "Transparent";

        private Vector2 pos;
        private Vector2 size;
        private TileType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile" /> class.
        /// </summary>
        /// <param name="tile">The tile this is based off of.</param>
        public Tile(Vector2 pos, Vector2 size, TiledLib.Tile tile)
        {
            this.pos = pos;
            this.size = size;

            Property tiletype;
            if (!tile.Properties.TryGetValue(TYPE_PROPERTY_NAME, out tiletype))
                throw new InvalidOperationException("Tile properties aren't valid.");
            else
            {
                // get the type of this tile from its property
                type = GetTypeByProperty(tiletype);
            }
        }

        private TileType GetTypeByProperty(Property type)
        {
            switch (type.ToString())
            {
                case SOLID:
                    return TileType.Solid;
                case TOP_SOLID:
                    return TileType.TopSolid;
                case TRANSPARENT:
                    return TileType.Transparent;
                default:
                    throw new InvalidOperationException("Invalid tile type name.");
            }
        }
    }

    public enum TileType
    {
        Solid,
        TopSolid,
        Transparent
    }
}
