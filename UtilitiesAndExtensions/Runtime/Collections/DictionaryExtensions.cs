using System.Collections.Generic;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities related to <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Allows you to get a key by value.
        /// It works by O(n), so use it knowing what you are doing.
        /// </summary>
        /// <param name="dictionary"> where keys and values will be taken from. </param>
        /// <param name="value"> The value that will be compared using <see cref="T:System.Collections.Generic.EqualityComparer`1"/>. </param>
        /// <returns> Returns the found key, or default. </returns>
        public static TKey KeyByValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value)
        {
            TKey needKey = default;
            foreach (var pair in dictionary)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                {
                    needKey = pair.Key;
                    break;
                }
            }
            return needKey;
        }
    }
}