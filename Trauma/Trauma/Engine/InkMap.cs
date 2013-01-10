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

        private const int MAX_SPLATTERS = 1000;
        #endregion

        #region Members

        private GraphicsDevice device;

        public RenderTarget2D Map;
        public Texture2D MapTex;

        private List<Splatter> splatters = new List<Splatter>();
        #endregion

        public InkMap(GraphicsDevice device, int width, int height)
        {
            this.device = device;
            Map = new RenderTarget2D(device, width, height, false, SurfaceFormat.Vector2, DepthFormat.None, 0,
                                     RenderTargetUsage.PreserveContents);
        }

        public void Update()
        {
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Map, Vector2.Zero, Color.White);
        }
    }
}
