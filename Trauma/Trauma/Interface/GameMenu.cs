using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trauma.Interface
{
    /// <summary>
    /// The in-game menu.
    /// </summary>
    public class GameMenu : Menu
    {
        public GameMenu() 
            : base(new Dictionary<string,Action>() 
            {
                {"Quit", () => {} }
            }, "Quit")
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
