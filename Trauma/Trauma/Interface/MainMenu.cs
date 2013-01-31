using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Trauma.Interface
{
    /// <summary>
    /// The main menu of the game.
    /// </summary>
    public class MainMenu : Menu
    {
        public MainMenu() : 
            base(new Dictionary<String, Action>() 
            {
                { "Quit", () => {} }
            }, "Quit") 
        {
        
        }
    }
}
