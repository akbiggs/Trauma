using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Trauma.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Trauma.Interface
{
    class Credits : IController
    {
        bool finished = false;
        private Color color = Color.Black;
        public void Initialize()
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            GameEngine.FadeIn(FadeSpeed.Slow);
            if (Input.KeyboardTapped(Keys.Enter) || Input.KeyboardTapped(Keys.Space))
                Finish();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.GraphicsDevice.Clear(Color.White);
            spriteBatch.Draw(ResourceManager.GetTexture("Misc_Credits"), Vector2.Zero, Color.White);
            //spriteBatch.GraphicsDevice.Clear(Color.LightGreen);
            //SpriteFont creditsFont = ResourceManager.GetFont("Credits");
            //String programmedBy = "Programmed By: Alexander Biggs";
            //Vector2 measurements = creditsFont.MeasureString(programmedBy);
            //spriteBatch.DrawString(creditsFont, programmedBy, 
            //    new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 200), color);

            //String drawnBy = "Designed/Illustrated By: Debbie Chan";
            //measurements = creditsFont.MeasureString(drawnBy);
            //spriteBatch.DrawString(creditsFont, drawnBy,
            //    new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 100), color);

            //String musicBy = "Music By: Kevin MacLeod (http://incompetech.com/)";
            //measurements = creditsFont.MeasureString(musicBy);
            //spriteBatch.DrawString(creditsFont, musicBy,
            //    new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 300), color);

            //String utgddc = "Created for the UTGDDC's GMD competition!";
            //measurements = creditsFont.MeasureString(utgddc);
            //spriteBatch.DrawString(creditsFont, utgddc,
            //   new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 400), color);

            //String thanks = "Thanks for playing!";
            //measurements = creditsFont.MeasureString(thanks);
            //spriteBatch.DrawString(creditsFont, thanks,
            //    new Vector2((spriteBatch.GraphicsDevice.Viewport.Width / 2) - measurements.X / 2, 500), color);

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
