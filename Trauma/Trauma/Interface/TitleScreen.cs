using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;

namespace Trauma.Interface
{
    /// <summary>
    /// The title screen of the game.
    /// Might not end up getting used...we'll see.
    /// </summary>
    public class TitleScreen : IController
    {
        private bool finished;

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public bool Finished
        {
            get { return finished; }
        }

        public void Finish()
        {
            finished = true;
        }

        public bool ExitSelected { get; set; }

        public void Update(GameTime gameTime)
        {
            // TODO: Title screen.
            Finish();
        }

    }
}
