using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Microsoft.Xna.Framework;

namespace Trauma.Interface
{
    /// <summary>
    /// The intro of the game.
    /// </summary>
    public class Intro : IController
    {
        bool finished = false;
        SpriteFont font;
        Texture2D logo;

        /// <summary>
        /// Make a new intro.
        /// </summary>
        public Intro()
        {

        }

        public void Initialize()
        {
            
        }

        /// <summary>
        /// Play the intro of the game.
        /// </summary>
        internal void Play()
        {
            // TODO: Implement intro.
            Finish();
        }

        public virtual bool Finished
        {
            get { return finished; }
        }

        public void Finish()
        {
            GameEngine.FadeOut(Color.White, FadeSpeed.Slow);
            finished = true;
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
