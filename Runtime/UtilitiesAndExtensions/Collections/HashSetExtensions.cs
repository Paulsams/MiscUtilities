using System.Collections.Generic;

namespace Paulsams.MicsUtil
{
    public static class HashSetExtensions
    {
        public static T First<T>(this HashSet<T> list)
        {
            var ienumertator = list.GetEnumerator();
            ienumertator.MoveNext();
            return ienumertator.Current;
        }
    }
}