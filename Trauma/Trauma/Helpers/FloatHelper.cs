using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trauma.Helpers
{
    public static class FloatHelper
    {
        #region Constants

        private const float FAST = 20;
        private const float MEDIUM = 10;
        private const float SLOW = 5;

        private const string FAST_STRING = "Fast";
        private const string MEDIUM_STRING = "Medium";
        private const string SLOW_STRING = "Slow";
        #endregion

        public static float ParseSpeedString(String speed)
        {
            switch (speed)
            {
                case FAST_STRING:
                    return FAST;
                case MEDIUM_STRING:
                    return MEDIUM;
                case SLOW_STRING:
                    return SLOW;
                default:
                    throw new InvalidOperationException("I don't know this speed you're specifying.");
            }
        }
    }
}
