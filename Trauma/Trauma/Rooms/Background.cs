﻿using System;
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
            spriteBatch.Draw(ResourceManager.GetTexture("Background_Intro"), new Rectangle(0, 0, 40 * 40, 40 * 40), null, Color.Red, 0,
                Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}
