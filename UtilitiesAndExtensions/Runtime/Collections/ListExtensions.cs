using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities related to <see cref="T:System.Collections.Generic.List`1"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Change the sheet size.
        /// If there was a larger size, then the elements at the end will be removed.
        /// If size was insufficient, then default elements will be added to the end.
        /// </summary>
        /// <param name="list"> Input list. </param>
        /// <param name="needSize"> The size that the sheet will have after executing this method. </param>
        public static void Resize<T>(this List<T> list, int needSize)
        {
            T defaultObject = default(T);
            int currentSize = list.Count;
            if (needSize < currentSize)
            {
                list.RemoveRange(needSize, currentSize - needSize);
            }
            else
            {
                if (needSize > list.Capacity)
                    list.Capacity = needSize;
                list.AddRange(Enumerable.Repeat(defaultObject, needSize - currentSize));
            }
        }

        /// <summary>
        /// If the <typeparamref name="T"/> type inherits from <see cref="T:System.ICloneable"/>,
        /// then it creates a new sheet and calls the <see cref="T:System.ICloneable.Clone"/> method on each element
        /// </summary>
        /// <returns> A new sheet where all the elements have been cloned. </returns>
        public static List<T> Clone<T>(this IEnumerable<T> source)
            where T : ICloneable =>
            source.Select(item => (T)item.Clone()).ToList();
    }
}