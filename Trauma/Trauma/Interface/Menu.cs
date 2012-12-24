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
    /// Basic generic menu.
    /// Offers options to player, waits for them to select one.
    /// </summary>
    public class Menu : IController
    {
        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public virtual bool Finished
        {
            get { throw new NotImplementedException();  }
        }

        public void Finish()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public virtual void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
