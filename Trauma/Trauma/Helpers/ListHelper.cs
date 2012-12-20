﻿using System.Collections.Generic;
using System.Diagnostics;

namespace Trauma.Helpers
{
    public static class ListHelper
    {
        /// <summary>
        /// Removes and returns a value from the specified list.
        /// </summary>
        /// <typeparam name="T">The type of the list</typeparam>
        /// <param name="list">The list to pop from.</param>
        /// <param name="index">The index of the value to pop.</param>
        /// <returns>The popped value.</returns>
        public static T Pop<T>(this List<T> list, int index=0) {
            Debug.Assert(index < list.Count, "List index out of range.");
            T popped = list[index];
            list.RemoveAt(index);
            return popped;
        }
    }
}
