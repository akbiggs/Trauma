using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Trauma.Helpers
{
    public static class VectorHelper
    {
        /// <summary>
        /// Push against the given vector by the given amount.
        /// </summary>
        /// <param name="vector">The vector to push.</param>
        /// <param name="push">The amount to push it by. Should not be negative.</param>
        /// <returns>The vector pushed back by the given amount, or the zero vector if the push amount
        /// was greater than the vector.</returns>
        public static Vector2 PushBack(this Vector2 vector, Vector2 push)
        {
            // push should be an absolute vector -- there is no negative push.
            Debug.Assert(Vector2.Max(push, Vector2.Zero) == push, "Negative push should not be applied.");

            // if going left, shove right, otherwise shove left.
            float newXComponent = vector.X < 0 ? Math.Min(vector.X + push.X, 0) : Math.Max(vector.X - push.X, 0);
            // similarly, if going up, push down, otherwise push up.
            float newYComponent = vector.Y < 0 ? Math.Min(vector.Y + push.Y, 0) : Math.Max(vector.Y - push.Y, 0);

            return new Vector2(newXComponent, newYComponent);
        }
    }
}
