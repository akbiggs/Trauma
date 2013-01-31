using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Microsoft.Xna.Framework;

namespace Trauma.Interface
{
    class Credits : IController
    {
        bool finished = false;

        public void Initialize()
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameEngine.FadeIn(FadeSpeed.Slow);
            if (Input.KeyboardTapped(Microsoft.Xna.Framework.Input.Keys.Enter))
                Finish();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            SpriteFont creditsFont = ResourceManager.GetFont("Credits");
            String programmedBy = "Programmed By: Alexander Biggs";
            Vector2 measurements = creditsFont.MeasureString(programmedBy);
            spriteBatch.DrawString(creditsFont, programmedBy, 
                new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 100), Color.Gray);

            String drawnBy = "Designed/Illustrated By: Debbie Chan";
            measurements = creditsFont.MeasureString(drawnBy);
            spriteBatch.DrawString(creditsFont, drawnBy,
                new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 200), Color.Gray);

            String thanks = "Thanks for playing!";
            measurements = creditsFont.MeasureString(thanks);
            spriteBatch.DrawString(creditsFont, thanks,
                new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 400), Color.Gray);

            spriteBatch.End();
        }

        public bool Finished
        {
            get { return finished; }
        }

        public void Finish()
        {
            finished = true;
        }

    }
}
