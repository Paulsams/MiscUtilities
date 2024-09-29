using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities related to <see cref="T:System.Collections.Generic.IEnumerable`1"/>.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Finds minimum vector by distance in collection from passed vector.
        /// </summary>
        /// <param name="source"> Input collection. </param>
        /// <param name="position"> The position from which minimum is calculated. </param>
        /// <param name="additionally"> Additional optional check implemented through a delegate. </param>
        /// <typeparam name="T"> Since it should be inherited from <see cref="T:UnityEngine.Component"/>, I take <see cref="P:UnityEngine.Transform.position"/>. </typeparam>
        public static T FindMinDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, T, bool> additionally = null) where T : Component =>
            FindMinDistanceElementOrDefaultInternal(source, position, component => component.transform.position,
                additionally);

        /// <summary>
        /// Finds maximum vector by distance in collection from passed vector.
        /// </summary>
        /// <param name="source"> Input collection. </param>
        /// <param name="position"> The position from which maximum is calculated. </param>
        /// <param name="additionally"> Additional optional check implemented through a delegate. </param>
        /// <typeparam name="T"> Since it should be inherited from <see cref="T:UnityEngine.Component"/>, I take <see cref="P:UnityEngine.Transform.position"/>. </typeparam>
        public static T FindMaxDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, T, bool> additionally = null) where T : Component =>
            FindMaxDistanceElementOrDefaultInternal(source, position, component => component.transform.position,
                additionally);

        /// <summary>
        /// Finds minimum vector by distance in collection from passed vector.
        /// </summary>
        /// <param name="source"> Input collection. </param>
        /// <param name="position"> The position from which minimum is calculated. </param>
        /// <param name="getVector"> A delegate that will return position associated with <typeparamref name="T"/>. </param>
        /// <param name="additionally"> Additional optional check implemented through a delegate. </param>
        /// <typeparam name="T"> Any type from which position is taken using <paramref name="getVector"/>. </typeparam>
        public static T FindMinDistanceElementOrDefault<T>(this IEnumerable<T> source, Vector3 position,
            Func<T, Vector3> getVector, Func<T, T, bool> additionally = null) =>
            FindMinDistanceElementOrDefaultInternal(source, position, getVector, additionally);

        /// <summary>
        /// Finds maximum vector by distance in collection from passed vector.
        /// </summary>
        /// <param name="source"> Input collection. </param>
        /// <param name="position"> The position from which maximum is calculated. </param>
        /// <param name="getVector"> A delegate that will return position associated with <typeparamref name="T"/>. </param>
        /// <param name="additionally"> Additional optional check implemented through a delegate. </param>
        /// <typeparam name="T"> Any type from which position is taken using <paramref name="getVector"/>. </typeparam>
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