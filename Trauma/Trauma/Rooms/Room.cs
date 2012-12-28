using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        // used to find the layer in the map with all of the tiles
        private const string TILE_LAYER_NAME = "Tiles";

        private const float DEFAULT_GRAVITY = 1f;

        // limit the amount of blobs in the room (or maybe the section of the room) for
        // sanity
        private const int MAX_NUM_BLOBS = 200;
        #endregion

        #region Members
        // used for collisions to shove some things just outside of walls
        private float Epsilon
        {
            get { return 0.001f; }
        }

        public bool MenuRequested = false;
        
        private bool finished = false;

        /* Actual room information */
        private Background background;
        private Map map;
        private int width;
        private int height;
        private Vector2 StageBounds
        {
            get { return new Vector2(width - Epsilon * 2, height - Epsilon); }
        }
        private Color color;
        private Vector2 tilesize;
        private Tile[,] tiles; 
        private List<Section> sections = new List<Section>();
        private Section curSection;
        public readonly float Gravity;

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
        /// <param name="gravity">The gravity of the room.</param>
        public Room(Color color, Map map, float gravity=DEFAULT_GRAVITY)
        {
            background = new Background();
            this.color = color;
            Gravity = gravity;
            this.map = map;

            width = map.WidthInPixels();
            height = map.HeightInPixels();
            
            /* Step 1: Load tiles from map. */
            tilesize = new Vector2(map.TileWidth, map.TileHeight);
            Debug.Assert(Math.Abs(map.WidthInPixels() % tilesize.X) < float.Epsilon, "Map doesn't divide evenly into rows of tiles.");
            Debug.Assert(Math.Abs(map.HeightInPixels() % tilesize.Y) < float.Epsilon, "Map doesn't divide evenly into columns of tiles.");

            tiles = new Tile[map.Height, map.Width];
            TileGrid grid = ((TileLayer) map.GetLayer(TILE_LAYER_NAME)).Tiles;
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                {
                    TiledLib.Tile tile = grid[x, y];
                    tiles[y, x] = new Tile(new Vector2(x * tilesize.X, y * tilesize.Y), tilesize, tile);
                }

            /* Step 2: Load objects from map. */
            MapObject playerobj = map.FindObject((layer, obj) => obj.Name == "Player");
            Add(new Player(new Vector2(playerobj.Bounds.X, playerobj.Bounds.Y)));

            // TODO: Load portal, ink generators.
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
                {
                    player = (Player) obj;
                    camera = new Camera(player);
                }
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
        /// Finish(exit) the room.
        /// </summary>
        public void Finish()
        {
            finished = true;
        }

        /// <summary>
        /// Updates the room.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Update(GameTime gameTime)
        {
            // Add and remove any buffered objects.
            AddAllBuffered();
            RemoveAllBuffered();

            background.Update(gameTime);

            // Update all objects in the room.
            player.Update(this, gameTime);
            //portal.Update(this, gameTime);
            //foreach (InkGenerator generator in generators)
            //    generator.Update(this, gameTime);
            //foreach (InkBlob blob in blobs)
            //    blob.Update(this, gameTime);

            // now that we've updated all the objects, update the camera.
            camera.Update(this, gameTime);
        }

        /// <summary>
        /// Draws the room.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Use camera to modify where things are drawn.
            spriteBatch.Begin(SpriteSortMode.BackToFront, 
                BlendState.AlphaBlend,
                null,
                null,
                null,
                null,
                camera.GetTransformation(spriteBatch.GraphicsDevice));

            background.Draw(spriteBatch);

            map.Draw(spriteBatch);
            //foreach (InkGenerator generator in generators)
            //    generator.Draw(spriteBatch);
            //portal.Draw(spriteBatch);
            player.Draw(spriteBatch);
            //foreach (InkBlob blob in blobs)
            //    blob.Draw(spriteBatch);

            spriteBatch.End();
        }

        /// <summary>
        /// Gets the minimum position of the given object.
        /// </summary>
        /// <param name="position">The position of the object.</param>
        /// <param name="size">The size of the object.</param>
        /// <returns>Vector2.</returns>
        public virtual Vector2 GetMinPosition(Vector2 position, Vector2 size)
        {
            Vector2 tilespan = GetTilespan(position, size);
            
            // the current tile index being examined
            Vector2 curTileIndex = GetTileIndexByPixel(position);
            bool done;
            // calculate x-bound
            float boundX = 0;
            done = false;
            for (int x = (int) curTileIndex.X; x >= 0; x--)
            {
                for (int y = (int) curTileIndex.Y; y <= curTileIndex.Y + tilespan.Y; y++)
                    if (tiles[y, x].Type == TileType.Solid)
                    {
                        boundX = x*tilesize.X + tilesize.X + Epsilon;
                        done = true;
                        break;
                    }
                if (done)
                    break;
            }

            // calculate y-bound
            float boundY = 0;
            done = false;
            for (int y = (int)curTileIndex.Y; y >= 0; y--)
            {
                for (int x = (int)curTileIndex.X; x <= curTileIndex.X + tilespan.X; x++)
                    if (tiles[y, x].Type == TileType.Solid)
                    {
                        boundY = y*tilesize.Y + tilesize.Y + Epsilon;
                        done = true;
                        break;
                    }
                if (done)
                    break;
            }
                
            return new Vector2(boundX, boundY);
        }

        public virtual Vector2 GetMaxPosition(Vector2 position, Vector2 size)
        {
            Vector2 tilespan = GetTilespan(position, size);

            Vector2 curTileIndex = GetTileIndexByPixel(position);
            bool done;

            // calculate x-bound
            float boundX = StageBounds.X - size.X;
            done = false;
            for (int x = (int)curTileIndex.X; x < map.Width; x++)
            {
                for (int y = (int)curTileIndex.Y; y <= curTileIndex.Y + tilespan.Y; y++)
                    if (tiles[y, x].Type == TileType.Solid)
                    {
                        boundX = x * tilesize.X - size.X - Epsilon;
                        done = true;
                        break;
                    }
                if (done)
                    break;
            }


            // calculate y-bound
            float boundY = StageBounds.Y - size.Y;
            done = false;
            for (int y = (int)curTileIndex.Y; y < map.Height; y++)
            {
                for (int x = (int)curTileIndex.X; x <= curTileIndex.X + tilespan.X; x++)
                    if (tiles[y, x].Type == TileType.Solid)
                    {
                        boundY = y * tilesize.Y - size.Y - Epsilon;
                        done = true;
                        break;
                    }
                if (done)
                    break;
            }

            return new Vector2(boundX, boundY);
        }

        /// <summary>
        /// Gets the tilespan of a object.
        /// </summary>
        /// <param name="position">The position of the object.</param>
        /// <param name="size">The size of the object.</param>
        /// <returns>How many tiles the object spans on the x-axis and y-axis(not including the one the position is on).</returns>
        private Vector2 GetTilespan(Vector2 position, Vector2 size)
        {
            Tile startTile = GetTileByPixel(Vector2.Clamp(position, Vector2.Zero, StageBounds));
            Tile endTile = GetTileByPixel(Vector2.Clamp(position + size, Vector2.Zero, StageBounds));
            float spanX = (endTile.Position.X - startTile.Position.X)/tilesize.X;
            float spanY = (endTile.Position.Y - startTile.Position.Y)/tilesize.Y;
            return new Vector2(spanX, spanY);
        }

        /// <summary>
        /// Gets the tile corresponding to the given pixel.
        /// </summary>
        /// <param name="position">The pixel position.</param>
        /// <returns>The tile at the given position.</returns>
        public Tile GetTileByPixel(Vector2 position)
        {
            if (InBounds(position))
                return tiles[(int) (position.Y/tilesize.Y), (int) (position.X/tilesize.X)];
            
            return null;
        }

        public Vector2 GetTileIndexByPixel(Vector2 position)
        {
            if (InBounds(position))
                return new Vector2((int) (position.X/tilesize.X), (int) (position.Y/tilesize.Y));

            return new Vector2(-1, -1);
        }

        private bool InBounds(Vector2 position)
        {
            return position == Vector2.Clamp(position, Vector2.Zero, StageBounds);
        }

        public void Splat(Vector2 position, Vector2 size, Color splatColor, Vector2 velocity)
        {
            // TODO: Add a splat to the room.
        }

        public bool CanHaveMoreBlobs()
        {
            return blobs.Count < MAX_NUM_BLOBS;
        }
    }
}