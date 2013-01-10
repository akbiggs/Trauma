using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;

namespace Trauma.Interface
{
    class Credits : IController
    {
        bool finished = false;

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }

        public bool Finished
        {
            get { return finished; }
        }

        public void Finish()
        {
            finished = true;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Finish();
        }
    }
}
