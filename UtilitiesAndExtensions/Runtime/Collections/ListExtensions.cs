using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static List<T> Clone<T>(this IEnumerable<T> source)
            where T : ICloneable =>
            source.Select(item => (T)item.Clone()).ToList();

        public static T FindMinDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, T, bool> additionally = null) where T : Component =>
            FindMinDistanceElementOrDefaultInternal(source, position, (component) => component.transform.position,
                additionally);

        public static T FindMaxDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, T, bool> additionally = null) where T : Component =>
            FindMaxDistanceElementOrDefaultInternal(source, position, (component) => component.transform.position,
                additionally);

        public static T FindMinDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, Vector3> getVector, Func<T, T, bool> additionally = null) =>
            FindMinDistanceElementOrDefaultInternal(source, position, getVector, additionally);

        public static T FindMaxDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, Vector3> getVector, Func<T, T, bool> additionally = null) =>
            FindMaxDistanceElementOrDefaultInternal(source, position, getVector, additionally);

        private static T FindMinDistanceElementOrDefaultInternal<T>(IEnumerable<T> source, Vector3 position,
            Func<T, Vector3> getVector, Func<T, T, bool> additionally) =>
            FindElementFromDistanceOrDefault(source, position, (first, second) => first < second, float.MaxValue,
                getVector, additionally);

        private static T FindMaxDistanceElementOrDefaultInternal<T>(IEnumerable<T> source, Vector3 position,
            Func<T, Vector3> getVector, Func<T, T, bool> additionally) =>
            FindElementFromDistanceOrDefault(source, position, (first, second) => first > second, float.MinValue,
                getVector, additionally);

        private static T FindElementFromDistanceOrDefault<T>(IEnumerable<T> source, Vector3 position,
            Func<float, float, bool> predicate, float startValue, Func<T, Vector3> getVector,
            Func<T, T, bool> additionally)
        {
            float sqrDistanceToNearestElement = startValue;
            T optimalElement = default;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void TryUpdateNearest(in T elem)
            {
                float sqrDistanceToTempObject = (position - getVector(elem)).sqrMagnitude;
                if (predicate(sqrDistanceToTempObject, sqrDistanceToNearestElement) &&
                    (additionally == null || additionally.Invoke(optimalElement, elem)))
                {
                    sqrDistanceToNearestElement = sqrDistanceToTempObject;
                    optimalElement = elem;
                }
            }

            if (source is IList<T> list)
            {
                for (var i = 0; i < list.Count; i++)
                    TryUpdateNearest(list[i]);
            }
            else
            {
                foreach (var elem in source)
                    TryUpdateNearest(elem);
            }

            return optimalElement;
        }
    }
}