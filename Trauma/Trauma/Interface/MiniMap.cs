using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Trauma.Objects;
using Trauma.Rooms;

namespace Trauma.Interface
{
    class MiniMap
    {
        #region Constants
        private const float SCALE_X_FACTOR = 0.05f;
        private const float SCALE_Y_FACTOR = 0.05f;

        #endregion

        #region Members
        private Vector2 position;

        private int roomWidth;
        private int roomHeight;

        private int mapWidth;
        private int mapHeight;

        public Dictionary<GameObject, Texture2D> Mappings = new Dictionary<GameObject, Texture2D>(); 
        #endregion

        public MiniMap(Room room, Vector2 position)
        {
            this.position = position;
            roomWidth = room.Width;
            roomHeight = room.Height;

            mapWidth = (int)MathHelper.Lerp(0, roomWidth, SCALE_X_FACTOR);
            mapHeight = (int)MathHelper.Lerp(0, roomHeight, SCALE_Y_FACTOR);
        }

        public void Update(Room room, GameTime gameTime)
        {
            
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            RenderBase(spriteBatch);

            foreach (KeyValuePair<GameObject, Texture2D> iconPair in Mappings)
            {
                GameObject obj = iconPair.Key;
                Texture2D tex = iconPair.Value;
                spriteBatch.Draw(tex, position + GetMapPosition(obj), obj is Player ? Color.White : obj.Color);
            }
        }

        private void RenderBase(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ResourceManager.GetTexture("Misc_Pixel"), GetMapRectangle(), new Color(0, 0, 0, 200));
        }

        public Vector2 GetMapPosition(GameObject obj)
        {
            return obj.Position * Scale;
        }

        private Rectangle GetMapRectangle()
        {
            return new Rectangle((int) position.X, (int) position.Y, mapWidth, mapHeight);
        }

        public Vector2 Scale
        {
            get { return new Vector2((float)mapWidth / roomWidth, (float)mapHeight / roomHeight); }
        }
    }
}
