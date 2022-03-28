using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtil
{
    public static class ReflectionExtensions
    {
        public static T[] GetPublicConstFields<T>(this Type type)
        {
            return GetPublicStaticFields<T>(type, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                field => field.IsLiteral && field.IsInitOnly == false && field.FieldType == typeof(T),
                (field) => (T)field.GetRawConstantValue());
        }

        public static T[] GetPublicStaticReadonlyFields<T>(this Type type)
        {
            return GetPublicStaticFields<T>(type, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                (field) => field.IsLiteral == false && field.IsInitOnly && field.FieldType == typeof(T),
                (field) => (T)field.GetValue(null));
        }

        public static T[] GetPublicStaticFields<T>(this Type type, BindingFlags bindingFlags, Func<FieldInfo, bool> predicate, Func<FieldInfo, T> selector)
        {
            return type.GetFields(bindingFlags)
                   .Where(predicate).Select(selector).ToArray();
        }
    }
}