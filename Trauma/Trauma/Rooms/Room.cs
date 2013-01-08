using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledLib;
using Trauma.Engine;
using Trauma.Helpers;
using Trauma.Interface;
using Trauma.Objects;

namespace Trauma.Rooms
{
    /// <summary>
    ///     A room of the game.
    ///     At any point, all the action is taking place in one room.
    ///     Rooms are not revisited.
    /// </summary>
    public class Room : IController
    {
        #region Constants

        // used to identify the player on the map
        private const string PLAYER_OBJECT_NAME = "Player";

        // used to identify generators
        private const string GENERATOR_OBJECT_NAME = "Generator";
        private const string COLOR_PROPERTY_NAME = "Color";
        private const string DIRECTION_PROPERTY_NAME = "Direction";
        private const string INTERVAL_PROPERTY_NAME = "Interval";
        private const string SPEED_PROPERTY_NAME = "Speed";

        // used to find the layer in the map with all of the tiles
        private const string TILE_LAYER_NAME = "Tiles";

        private const float DEFAULT_GRAVITY = 1f;

        private const float DEFAULT_GENERATOR_VELOCITY = 15;
        private const int DEFAULT_GENERATOR_INTERVAL = 20;

        // can be used to determine splatter proportions based on object speed
        private const float SPLATTER_SIZE_FACTOR = 0.1f;

        // limit the amount of blobs in the room (or maybe the section of the room) for sanity
        private const int MAX_NUM_BLOBS = 200;

        #endregion

        #region Members

        /* Actual room information */
        public readonly float Gravity;
        private readonly Background background;
        private readonly List<InkBlob> blobs = new List<InkBlob>();
        private readonly List<InkGenerator> generators = new List<InkGenerator>();
        private readonly int height;
        private readonly InkMap inkMap;
        private readonly Map map;

        private Tile[,] tiles;

        private List<GameObject> toAdd = new List<GameObject>();
        private List<GameObject> toRemove = new List<GameObject>();

        private readonly int width;
        public bool MenuRequested;
        private Camera camera;
        private Color color;
        private Section curSection;
        private List<Dialogue> dialogues = new List<Dialogue>();
        private bool finished;

        private Player player;
        private Portal portal;
        private List<Section> sections = new List<Section>();
        private Vector2 tilesize;

        // used for collisions to shove some things just outside of walls
        private float Epsilon
        {
            get { return 0.001f; }
        }

        private Vector2 StageBounds
        {
            get { return new Vector2(width - Epsilon*2, height - Epsilon); }
        }

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="Room" /> class.
        /// </summary>
        /// <param name="color">The color of the map.</param>
        /// <param name="map">The map this room is based on.</param>
        /// <param name="gravity">The gravity of the room.</param>
        public Room(Color color, Map map, float gravity = DEFAULT_GRAVITY)
        {
            this.color = color;
            Gravity = gravity;
            this.map = map;
            inkMap = new InkMap();

            width = map.WidthInPixels();
            height = map.HeightInPixels();
            background = new Background(width, height);

            /* Step 1: Load tiles from map. */
            tilesize = new Vector2(map.TileWidth, map.TileHeight);
            Debug.Assert(Math.Abs(map.WidthInPixels()%tilesize.X) < float.Epsilon,
                         "Map doesn't divide evenly into rows of tiles.");
            Debug.Assert(Math.Abs(map.HeightInPixels()%tilesize.Y) < float.Epsilon,
                         "Map doesn't divide evenly into columns of tiles.");

            tiles = new Tile[map.Height,map.Width];
            TileGrid grid = ((TileLayer) map.GetLayer(TILE_LAYER_NAME)).Tiles;
            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                {
                    TiledLib.Tile tile = grid[x, y];
                    tiles[y, x] = new Tile(new Vector2(x*tilesize.X, y*tilesize.Y), tilesize, tile);
                }

            /* Step 2: Load objects from map. */
            MapObject playerObj = map.FindObject((layer, obj) => obj.Name == PLAYER_OBJECT_NAME);
            Add(new Player(new Vector2(playerObj.Bounds.X, playerObj.Bounds.Y)));

            IEnumerable<MapObject> generatorObjs = map.FindObjects((layer, obj) => obj.Type == GENERATOR_OBJECT_NAME);
            foreach (MapObject generatorObj in generatorObjs)
            {
                // get all the properties of the generator from the map object
                Property directionProperty;
                Debug.Assert(generatorObj.Properties.TryGetValue(DIRECTION_PROPERTY_NAME, out directionProperty),
                             "Generator with no direction.");
                Vector2? genDirection = VectorHelper.FromDirectionString(directionProperty.RawValue);
                Debug.Assert(genDirection != null, "Invalid direction name specified.");

                float genX = genDirection.Value.X > 0 ? generatorObj.Bounds.Right : generatorObj.Bounds.Left;
                float genY = genDirection.Value.Y < 0 ? generatorObj.Bounds.Bottom : generatorObj.Bounds.Top;

                Property colorProperty;
                Debug.Assert(generatorObj.Properties.TryGetValue(COLOR_PROPERTY_NAME, out colorProperty),
                             "Generator with no color.");
                Color? genColor = ColorHelper.FromString(colorProperty.RawValue);
                Debug.Assert(genColor != null, "Invalid color was specified.");

                int genInterval = DEFAULT_GENERATOR_INTERVAL;
                Property intervalProperty;
                if (generatorObj.Properties.TryGetValue(INTERVAL_PROPERTY_NAME, out intervalProperty))
                    genInterval = int.Parse(intervalProperty.RawValue);

                float genSpeed = DEFAULT_GENERATOR_VELOCITY;
                Property speedProperty;
                if (generatorObj.Properties.TryGetValue(SPEED_PROPERTY_NAME, out speedProperty))
                    genSpeed = FloatHelper.ParseSpeedString(speedProperty.RawValue);

                Add(new InkGenerator(new Vector2(genX, genY), genDirection.Value, genColor.Value, genInterval, genSpeed));
            }

            // TODO: Load portal.
        }

        /// <summary>
        ///     Whether or not the room is completed.
        /// </summary>
        public bool Finished
        {
            get { return finished; }
        }

        /// <summary>
        ///     Finish(exit) the room.
        /// </summary>
        public void Finish()
        {
            finished = true;
        }

        /// <summary>
        ///     Updates the room.
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
            foreach (InkGenerator generator in generators)
                generator.Update(this, gameTime);
            foreach (InkBlob blob in blobs)
                blob.Update(this, gameTime);

            // handle any collisions with the player
            BBox collision;
            //if (portal.IsColliding(player, out collision))
            //{
            //    player.CollideWithObject(portal, this, collision);
            //    portal.CollideWithObject(player, this, collision);
            //}
            foreach (InkGenerator generator in generators)
                if (generator.IsColliding(player, out collision))
                {
                    player.CollideWithObject(generator, this, collision);
                    generator.CollideWithObject(player, this, collision);
                }
            foreach (InkBlob blob in blobs)
                if (blob.IsColliding(player, out collision))
                {
                    player.CollideWithObject(blob, this, collision);
                    blob.CollideWithObject(player, this, collision);
                }

            // now that we've handled all those objects, update the camera to track whatever it wants to track.
            camera.Update(this, gameTime);
            inkMap.Update();
        }

        /// <summary>
        ///     Draws the room.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront,
                              BlendState.AlphaBlend,
                              null,
                              null,
                              null,
                              null,
                              camera.GetTransformation(spriteBatch.GraphicsDevice));

            background.Draw(spriteBatch);

            map.Draw(spriteBatch);
            inkMap.Draw(spriteBatch);
            foreach (InkGenerator generator in generators)
                generator.Draw(spriteBatch);
            //portal.Draw(spriteBatch);
            player.Draw(spriteBatch);
            foreach (InkBlob blob in blobs)
                blob.Draw(spriteBatch);

            spriteBatch.End();
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

            toAdd = new List<GameObject>();
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
                    generators.Remove((InkGenerator) obj);
                else if (obj is InkBlob)
                    blobs.Remove((InkBlob) obj);
            }

            toRemove = new List<GameObject>();
        }

        /// <summary>
        ///     Gets the minimum position of the given object.
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
            for (var x = (int) curTileIndex.X; x >= 0; x--)
            {
                for (var y = (int) curTileIndex.Y; y <= curTileIndex.Y + tilespan.Y; y++)
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
            for (var y = (int) curTileIndex.Y; y >= 0; y--)
            {
                for (var x = (int) curTileIndex.X; x <= curTileIndex.X + tilespan.X; x++)
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
            for (var x = (int) curTileIndex.X; x < map.Width; x++)
            {
                for (var y = (int) curTileIndex.Y; y <= curTileIndex.Y + tilespan.Y; y++)
                    if (tiles[y, x].Type == TileType.Solid)
                    {
                        boundX = x*tilesize.X - size.X - Epsilon;
                        done = true;
                        break;
                    }
                if (done)
                    break;
            }


            // calculate y-bound
            float boundY = StageBounds.Y - size.Y;
            done = false;
            for (var y = (int) curTileIndex.Y; y < map.Height; y++)
            {
                for (var x = (int) curTileIndex.X; x <= curTileIndex.X + tilespan.X; x++)
                    if (tiles[y, x].Type == TileType.Solid)
                    {
                        boundY = y*tilesize.Y - size.Y - Epsilon;
                        done = true;
                        break;
                    }
                if (done)
                    break;
            }

            return new Vector2(boundX, boundY);
        }

        /// <summary>
        ///     Gets the tilespan of a object.
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

        public bool WallAt(Vector2 pixel)
        {
            return GetTileByPixel(pixel).Type == TileType.Solid;
        }

        public bool WallAtIndex(int col, int row)
        {
            Debug.Assert(0 <= col && col < map.Height, "Column out of range.");
            Debug.Assert(0 <= row && row < map.Width, "Row out of range.");
            return GetTileByPixel(new Vector2(row*tilesize.X, col*tilesize.Y)).Type == TileType.Solid;
        }

        /// <summary>
        ///     Gets the tile corresponding to the given pixel.
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
            // rotate the splatter with the velocity of the angle, but don't flip vertically.
            float rotation = (new Vector2(velocity.X, -Math.Abs(velocity.Y))).ToAngle();

            Vector2 splatSize;

            // only acccomodate for the velocity if it's non-zero. Also, need to take the absolute value to avoid
            // getting negative size values in return (that's invalid behavior).
            if (Math.Abs(velocity.X) > 0)
                splatSize.X = size.X*Math.Abs(velocity.X)*SPLATTER_SIZE_FACTOR;
            else splatSize.X = size.X*SPLATTER_SIZE_FACTOR;

            if (Math.Abs(velocity.Y) > 0)
                splatSize.Y = size.Y*Math.Abs(velocity.Y)*SPLATTER_SIZE_FACTOR;
            else splatSize.Y = size.Y*SPLATTER_SIZE_FACTOR;

            Vector2 splatPos = position;

            if (velocity.X > 0)
                splatPos.X -= size.X;
            if (velocity.Y > 0)
                splatPos.Y += size.Y * (3/4);
            // if one of the components is huge but the other is really small, the splatter will look weird, so balance
            // it out.
            splatSize = splatSize.Balance();

            inkMap.AddSplatter(splatPos, splatSize, rotation, ResourceManager.GetRandomSplatter(), splatColor);
        }

        public bool CanHaveMoreBlobs()
        {
            return blobs.Count < MAX_NUM_BLOBS;
        }
    }
}