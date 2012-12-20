using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TiledLib;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Interface;
using Trauma.Objects;
using System.Diagnostics;

namespace Trauma.Rooms
{
    /// <summary>
    /// A room of the game.
    /// At any point, all the action is taking place in one room.
    /// Rooms are not revisited.
    /// </summary>
    public class Room : IController
    {
        #region Constants
        
        // used to identify the player on the map
        private const string PLAYER_OBJECT_NAME = "Player";
        
        #endregion

        #region Members

        public bool MenuRequested = false;
        
        private bool finished = false;

        /* Actual room information */
        private Background background;
        private Map map;
        private int width;
        private int height;
        private Color color;
        private Vector2 tilesize;
        private Tile[,] tiles; 
        private List<Section> sections = new List<Section>();
        private Section curSection;

        private Camera camera;

        /* Room objects */
        private List<GameObject> toAdd = new List<GameObject>();
        private List<GameObject> toRemove = new List<GameObject>(); 

        private Player player;
        private Portal portal;

        private InkMap inkMap;
        private List<InkGenerator> generators = new List<InkGenerator>();
        private List<InkBlob> blobs = new List<InkBlob>();

        private List<Dialogue> dialogues = new List<Dialogue>();

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Room" /> class.
        /// </summary>
        /// <param name="color">The color of the map.</param>
        /// <param name="map">The map this room is based on.</param>
        public Room(Color color, Map map)
        {
            this.color = color;
            this.map = map;
            
            /* Step 1: Load tiles from map. */
            tilesize = new Vector2(map.TileWidth, map.TileHeight);
            Debug.Assert(Math.Abs(map.Width % tilesize.X) < float.Epsilon, "Map doesn't divide evenly into rows of tiles.");
            Debug.Assert(Math.Abs(map.Height % tilesize.Y) < float.Epsilon, "Map doesn't divide evenly into columns of tiles.");
            for (int y = 0; y < map.HeightInTiles(); y++)
                for (int x = 0; x < map.WidthInTiles(); x++)
                {
                    TiledLib.Tile tile = map.Tiles[x + y*map.WidthInTiles()];
                    tiles[y, x] = new Tile(new Vector2(x * tilesize.X, y * tilesize.Y), tilesize, tile);
                }

            /* Step 2: Load player from map. */
            MapObject playerobj = map.FindObject((layer, obj) => obj.Name == "Player");
            Add(new Player(new Vector2(playerobj.Bounds.X, playerobj.Bounds.Y));
        }

        public void Add(GameObject obj)
        {
            toAdd.Add(obj);
        }

        private void AddAllBuffered()
        {
            foreach (GameObject obj in toAdd)
            {
                if (obj is Player)
                    player = (Player) obj;
                else if (obj is Portal)
                    portal = (Portal) obj;
                else if (obj is InkGenerator)
                    generators.Add((InkGenerator) obj);
                else if (obj is InkBlob)
                    blobs.Add((InkBlob) obj);
            }
        }

        public void Remove(GameObject obj)
        {
            toRemove.Add(obj);
        }

        private void RemoveAllBuffered()
        {
            foreach (GameObject obj in toRemove)
            {
                if (obj is Player)
                    player = null;
                else if (obj is Portal)
                    portal = null;
                else if (obj is InkGenerator)
                    generators.Remove((InkGenerator)obj);
                else if (obj is InkBlob)
                    blobs.Remove((InkBlob) obj);
            }
        }

        /// <summary>
        /// Whether or not the room is completed.
        /// </summary>
        public bool Finished
        {
            get { return finished; }
        }

        /// <summary>
        /// Finish the room.
        /// </summary>
        public void Finish()
        {
            finished = true;
        }

        public void Update(GameTime gameTime)
        {
            // Update all objects in the room.
            player.Update(gameTime);
            portal.Update(gameTime);
            foreach (InkGenerator generator in generators)
                generator.Update();
            foreach (InkBlob blob in blobs)
                blob.Update();

            // Add and remove any buffered objects.
            AddAllBuffered();
            RemoveAllBuffered();
        }
    }
}
