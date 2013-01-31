using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Trauma.Interface
{
    public class Icon
    {
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }
        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
        }

        private Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
        }

        public Icon(Texture2D texture, Vector2 position, Vector2 size)
        {
            this.texture = texture;
            this.position = position;
            this.size = size;
        }

        public void Update()
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);
        }
    }
}
