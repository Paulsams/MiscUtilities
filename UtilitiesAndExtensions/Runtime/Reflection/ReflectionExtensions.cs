using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Extension methods for different classes, but they all involve reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Get all public constant fields of a class.
        /// </summary>
        /// <param name="type"> The class <see cref="T:System.Type"/> from which the field data will be taken. </param>
        /// <param name="checkForAssignableType"> Will types inherited from <typeparamref name="T"/> be returned? </param>
        public static IEnumerable<T> GetPublicConstFields<T>(this Type type, bool checkForAssignableType = false) =>
            GetPublicConstFields<T, T>(type, field => (T)field.GetRawConstantValue(), checkForAssignableType);

        /// <summary>
        /// Get all public constant fields of a class.
        /// </summary>
        /// <param name="type"> The class <see cref="T:System.Type"/> from which the field data will be taken. </param>
        /// <param name="converter"> A converter from FieldInfo to <typeparamref name="TResult"/>. </param>
        /// <param name="checkForAssignableType"> Will types inherited from <typeparamref name="TField"/> be returned? </param>
        public static IEnumerable<TResult> GetPublicConstFields<TField, TResult>(
            this Type type,
            Func<FieldInfo, TResult> converter,
            bool checkForAssignableType = false)
        {
            return GetPublicStaticFields<TField, TResult>(type, checkForAssignableType,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                field => field.IsLiteral && field.IsInitOnly == false,
                converter);
        }

        /// <summary>
        /// Get all public static readonly fields of a class.
        /// </summary>
        /// <param name="type"> The class <see cref="T:System.Type"/> from which the field data will be taken. </param>
        /// <param name="checkForAssignableType"> Will types inherited from <typeparamref name="T"/> be returned? </param>
        public static IEnumerable<T> GetPublicStaticReadonlyFields<T>(this Type type,
            bool checkForAssignableType = false) =>
            GetPublicStaticReadonlyFields<T, T>(type, field => (T)field.GetValue(null), checkForAssignableType);

        /// <summary>
        /// Get all public static readonly fields of a class.
        /// </summary>
        /// <param name="type"> The class <see cref="T:System.Type"/> from which the field data will be taken. </param>
        /// <param name="converter"> A converter from FieldInfo to <typeparamref name="TResult"/>. </param>
        /// <param name="checkForAssignableType"> Will types inherited from <typeparamref name="TField"/> be returned? </param>
        public static IEnumerable<TResult> GetPublicStaticReadonlyFields<TField, TResult>(this Type type,
            Func<FieldInfo, TResult> converter,
            bool checkForAssignableType = false)
        {
            return GetPublicStaticFields<TField, TResult>(type, checkForAssignableType,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                field => field.IsLiteral == false && field.IsInitOnly,
                converter);
        }

        private static IEnumerable<TResult> GetPublicStaticFields<TField, TResult>(
            this Type type,
            bool checkForAssignableType,
            BindingFlags bindingFlags,
            Func<FieldInfo, bool> predicate,
            Func<FieldInfo, TResult> selector)
        {
            return type.GetFields(bindingFlags)
                .Where(field => predicate(field) && (checkForAssignableType
                    ? typeof(TField).IsAssignableFrom(field.FieldType)
                    : field.FieldType == typeof(TField)))
                .Select(selector);
        }
    }
}