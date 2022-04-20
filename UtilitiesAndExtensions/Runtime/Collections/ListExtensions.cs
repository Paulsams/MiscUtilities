using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paulsams.MicsUtils
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

        public static T FindMinDistanceElementOrDefault<T>(this IList<T> list, Vector3 position, Func<T, T, bool> additionally = null) where T : Component =>
            FindMinDistanceElementOrDefaultInternal(list, position, (component) => component.transform.position, additionally);

        public static T FindMaxDistanceElementOrDefault<T>(this IList<T> list, Vector3 position, Func<T, T, bool> additionally = null) where T : Component =>
            FindMaxDistanceElementOrDefaultInternal(list, position, (component) => component.transform.position, additionally);

        public static T FindMinDistanceElementOrDefault<T>(this IList<T> list, Vector3 position, Func<T, Vector3> getVector, Func<T, T, bool> additionally = null) =>
            FindMinDistanceElementOrDefaultInternal(list, position, getVector, additionally);

        public static T FindMaxDistanceElementOrDefault<T>(this IList<T> list, Vector3 position, Func<T, Vector3> getVector, Func<T, T, bool> additionally = null) =>
            FindMaxDistanceElementOrDefaultInternal(list, position, getVector, additionally);

        private static T FindMinDistanceElementOrDefaultInternal<T>(IList<T> list, Vector3 position, Func<T, Vector3> getVector, Func<T, T, bool> additionally) =>
            FindElementFromDistanceOrDefault(list, position, (first, second) => first < second, float.MaxValue, getVector, additionally);

        private static T FindMaxDistanceElementOrDefaultInternal<T>(IList<T> list, Vector3 position, Func<T, Vector3> getVector, Func<T, T, bool> additionally) =>
            FindElementFromDistanceOrDefault(list, position, (first, second) => first > second, float.MinValue, getVector, additionally);

        private static T FindElementFromDistanceOrDefault<T>(IList<T> list, Vector3 position, Func<float, float, bool> predicate, float startValue, Func<T, Vector3> getVector, Func<T, T, bool> additionally)
        {
            if (list.Count == 0)
                return default;

            float sqrDistanceToNearestElement = startValue;
            T optimalElement = default;

            for (int i = 0; i < list.Count; ++i)
            {
                float sqrDistanceToTempObject = (position - getVector(list[i])).sqrMagnitude;
                if (predicate(sqrDistanceToTempObject, sqrDistanceToNearestElement) && (additionally == null || additionally.Invoke(optimalElement, list[i])))
                {
                    sqrDistanceToNearestElement = sqrDistanceToTempObject;
                    optimalElement = list[i];
                }
            }

            return optimalElement;
        }
    }
}