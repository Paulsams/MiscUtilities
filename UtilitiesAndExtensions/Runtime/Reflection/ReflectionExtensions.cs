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
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<T> GetPublicConstFields<T>(this Type type, bool checkForAssignableType = false)
        {
            return GetPublicStaticFields(type, checkForAssignableType,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                field => field.IsLiteral && field.IsInitOnly == false,
                (field) => (T)field.GetRawConstantValue());
        }

        /// <summary>
        /// Get all public static reaonly fields of a class.
        /// </summary>
        /// <param name="type"> The class <see cref="T:System.Type"/> from which the field data will be taken. </param>
        /// <param name="checkForAssignableType"> Will types inherited from <typeparamref name="T"/> be returned? </param>
        /// <typeparam name="T"></typeparam>
        public static IEnumerable<T> GetPublicStaticReadonlyFields<T>(this Type type, bool checkForAssignableType = false)
        {
            return GetPublicStaticFields(type, checkForAssignableType,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                (field) => field.IsLiteral == false && field.IsInitOnly,
                (field) => (T)field.GetValue(null));
        }

        /// <summary>
        /// Get all public static fields of a class.
        /// </summary>
        /// <param name="type"> The class <see cref="T:System.Type"/> from which the field data will be taken. </param>
        /// <param name="checkForAssignableType"> Will types inherited from <typeparamref name="T"/> be returned? </param>
        /// <typeparam name="T"></typeparam>
        private static IEnumerable<T> GetPublicStaticFields<T>(this Type type, bool checkForAssignableType,
            BindingFlags bindingFlags, Func<FieldInfo, bool> predicate, Func<FieldInfo, T> selector)
        {
            return type.GetFields(bindingFlags)
                .Where((field) => predicate(field) && (checkForAssignableType
                    ? typeof(T).IsAssignableFrom(field.FieldType)
                    : field.FieldType == typeof(T))).Select(selector);
        }
    }
}