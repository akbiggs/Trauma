using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;

namespace Trauma.Rooms
{
    /// <summary>
    /// The background of the room.
    /// </summary>
    public class Background
    {
        private int width, height;
        private RoomType type;
        public Background(RoomType type, int width, int height)
        {
            this.width = width;
            this.height = height;
            this.type = type;
        }
        /// <summary>
        /// Updates the background.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            String typeName = GetTypeName(type);
            spriteBatch.Draw(ResourceManager.GetTexture("Background_" + typeName), new Rectangle(0, 0, width, height), null, Color.White, 0,
                Vector2.Zero, SpriteEffects.None, 1);
        }

        private String GetTypeName(RoomType type)
        {
            switch (type)
            {
                case RoomType.Acceptance:
                    return "Acceptance";
                case RoomType.Anger:
                    return "Anger";
                case RoomType.Bargain:
                    return "Bargain";
                case RoomType.Denial:
                    return "Denial";
                case RoomType.Depression:
                    return "Depression";
                default:
                    throw new InvalidOperationException("Invalid room type.");
            }
        }
    }
}
