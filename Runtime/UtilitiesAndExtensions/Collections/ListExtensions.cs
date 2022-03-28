using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paulsams.MicsUtil
{
    public static class ListExtensions
    {
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

        public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static T FindMinDistanceElement<T>(this IList<T> list, Vector3 position, Func<Component, Component, bool> additionally = null) where T : Component =>
            FindElementFromDistance(list, position, (first, second) => first < second, additionally);

        public static T FindMaxDistanceElement<T>(this IList<T> list, Vector3 position, Func<Component, Component, bool> additionally = null) where T : Component =>
            FindElementFromDistance(list, position, (first, second) => first > second, additionally);

        private static T FindElementFromDistance<T>(this IList<T> list, Vector3 position, Func<float, float, bool> predicate, Func<Component, Component, bool> additionally) where T : Component
        {
            if (list.Count == 0)
                throw new ArgumentException("Collection is empty");

            float sqrDistanceToNearestElement = float.MaxValue;
            T optimalElement = null;

            for (int i = 0; i < list.Count; ++i)
            {
                float sqrDistanceToTempObject = (position - list[i].transform.position).sqrMagnitude;
                if (predicate(sqrDistanceToTempObject, sqrDistanceToNearestElement) && (additionally == null || additionally.Invoke(list[i], optimalElement)))
                {
                    sqrDistanceToNearestElement = sqrDistanceToTempObject;
                    optimalElement = list[i];
                }
            }

            if (optimalElement == null)
                throw new NullReferenceException("Not founded need element in find for distance");

            return optimalElement;
        }
    }
}