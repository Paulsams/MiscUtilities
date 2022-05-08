using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Paulsams.MicsUtils
{
    public static class ReflectionExtensions
    {
        public static T[] GetPublicConstFields<T>(this Type type, bool checkForAssignableType = false)
        {
            return GetPublicStaticFields<T>(type, checkForAssignableType, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                field => field.IsLiteral && field.IsInitOnly == false && field.FieldType == typeof(T),
                (field) => (T)field.GetRawConstantValue());
        }

        public static T[] GetPublicStaticReadonlyFields<T>(this Type type, bool checkForAssignableType = false)
        {
            return GetPublicStaticFields<T>(type, checkForAssignableType, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                (field) => field.IsLiteral == false && field.IsInitOnly && field.FieldType == typeof(T),
                (field) => (T)field.GetValue(null));
        }

        private static T[] GetPublicStaticFields<T>(this Type type, bool checkForAssignableType, BindingFlags bindingFlags, Func<FieldInfo, bool> predicate, Func<FieldInfo, T> selector)
        {
            return type.GetFields(bindingFlags)
                   .Where((field) => predicate(field) && (checkForAssignableType == false || typeof(T).IsAssignableFrom(field.FieldType))).Select(selector).ToArray();
        }
    }
}