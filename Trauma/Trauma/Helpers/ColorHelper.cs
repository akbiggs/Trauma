using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Trauma.Helpers
{
    public static class ColorHelper
    {
        #region Constants

        /* String representations */
        private const string RED = "Red";
        private const string BLUE = "Blue";
        private const string YELLOW = "Yellow";
        private const string ORANGE = "Orange";
        private const string PURPLE = "Purple";
        private const string GREEN = "Green";
        
        #endregion

        #region Members

        private static Color Red = Color.Red;
        private static Color Blue = Color.Blue;
        private static Color Yellow = Color.Yellow;
        private static Color Purple = Color.Purple;
        private static Color Orange = Color.DarkOrange;
        private static Color Green = Color.Green;
        #endregion

        public static Color? FromString(String name)
        {
            switch (name)
            {
                case RED:
                    return Red;
                case BLUE:
                    return Blue;
                case YELLOW:
                    return Yellow;
                case ORANGE:
                    return Orange;
                case PURPLE:
                    return Purple;
                case GREEN:
                    return Green;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns whether or not the given color contains the other color, i.e.
        /// is made from it.
        /// </summary>
        /// <param name="color">The color that might contain the other color.</param>
        /// <param name="containedColor">The color that might be contained within the other color.</param>
        /// <returns>True if the color is contained, false otherwise.</returns>
        public static bool Contains(this Color color, Color containedColor)
        {
            // black is contained in every color
            if (containedColor == Color.Black)
                return true;

            if (color == containedColor) 
                return true;

            if (color == Purple && (containedColor == Blue || containedColor == Red))
                return true;
            if (color == Orange && (containedColor == Red || containedColor == Yellow))
                return true;
            if (color == Green && (containedColor == Blue || containedColor == Yellow))
                return true;

            return false;
        }

        public static Color Combine(this Color color, Color otherColor)
        {

            // primary combinations
            if ((color == Red && otherColor == Blue) || (color == Blue && otherColor == Red))
                return Purple;
            if ((color == Blue && otherColor == Yellow) || (color == Yellow && otherColor == Blue))
                return Green;
            if ((color == Red && otherColor == Yellow) || (color == Yellow && otherColor == Red))
                return Orange;

            // redundant color combinations
            if (color.Contains(otherColor))
                return color;
            if (otherColor.Contains(color))
                return otherColor;

#if DEBUG
            Console.WriteLine(String.Format("Possibly unsupported color combination: {0} and {1}", color.ToString(), otherColor.ToString()));
#endif
            return Color.Black;
        }
    }
}
