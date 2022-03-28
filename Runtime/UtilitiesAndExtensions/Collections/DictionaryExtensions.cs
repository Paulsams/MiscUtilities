using System.Collections.Generic;

namespace Paulsams.MicsUtil
{
    public static class DictionaryExtensions
    {
        public static TKey KeyByValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TValue value)
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