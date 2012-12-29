using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Helpers;
using Trauma.Objects;

namespace Trauma.Engine
{
    /// <summary>
    /// Tracks and draws out how ink has been splattered onto the screen.
    /// </summary>
    public class InkMap
    {
        #region Constants

        private const int MAX_SPLATTERS = 150;
        #endregion

        #region Members
        /// <summary>
        /// Shows where it's okay to paint. White means don't paint, black means paintable.
        /// </summary>
        private Color[,] canvas;
        
        /// <summary>
        /// Stores the results of painting.
        /// </summary>
        private Texture2D map;

        private List<Splatter> splatters = new List<Splatter>();
        #endregion

        /// <summary>
        /// Adds a splatter to the map.
        /// </summary>
        /// <param name="position">The position of the splatter.</param>
        /// <param name="size">The size of the splatter.</param>
        /// <param name="rotation">The rotation of the splatter.</param>
        /// <param name="texture">The texture of the splatter.</param>
        /// <param name="color">The color of the splatter.</param>
        public void AddSplatter(Vector2 position, Vector2 size, float rotation, Texture2D texture, Color color)
        {
            if (splatters.Count + 1 > MAX_SPLATTERS)
                splatters.Pop(0);
            splatters.Add(new Splatter(position, size, rotation, texture, color));

            // TODO: Add this code back in when convinced that splats should be drawn only on solid surfaces.

            /* Step 1: Get color array from texture. */
            // Color[,] texColors = texture.ToColor2D();

            /* Step 2: Get transformation matrix from properties. */
            // Thanks Riemer! http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2D/Coll_Detection_Matrices.php


            /* Step 3: Transform texture accordingly. */

            /* Step 4: Paint splatter onto canvas -- set transparency = whiteness of color. */
        }

        public void Update()
        {
            foreach (Splatter splatter in splatters)
                splatter.Update();
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Splatter splatter in splatters)
                splatter.Draw(spriteBatch);
        }
    }
}
