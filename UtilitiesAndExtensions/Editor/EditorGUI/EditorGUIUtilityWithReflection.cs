using Paulsams.MicsUtils;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Paulsams.MicsUtils
{
    public static class EditorGUIUtilityWithReflection
    {
        private static readonly MethodInfo _hasVisibleChildFieldsMethod;
        private static readonly Type _scriptAttributeUtilityType;

        static EditorGUIUtilityWithReflection()
        {
            _hasVisibleChildFieldsMethod = typeof(EditorGUI).GetMethod("HasVisibleChildFields");
            
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "UnityEditor.CoreModule")
                {
                    _scriptAttributeUtilityType = assembly.GetType("UnityEditor.ScriptAttributeUtility");
                    break;
                }
            }
        }

        public static GenericMenu FillPropertyContextMenu(SerializedProperty property, SerializedProperty linkedProperty = null, GenericMenu menu = null)
            => typeof(EditorGUI).GetMethod("FillPropertyContextMenu",
                BindingFlags.NonPublic | BindingFlags.Static).
                Invoke(null, new object[] { property, linkedProperty, menu }) as GenericMenu;

        public static bool HasPropertyDrawer(SerializedProperty property)
        {
            var handler = _scriptAttributeUtilityType.GetMethod("GetHandler", 
                    BindingFlags.Static | BindingFlags.NonPublic).
                Invoke(null, new object[] { property });
            var handlerType = handler.GetType();
            using (handlerType.GetMethod("ApplyNestingContext", BindingFlags.Instance | BindingFlags.Public).
                       Invoke(handler, new object[] { 0 } ) as IDisposable)
            {
                return (bool)handlerType.GetProperty("hasPropertyDrawer", 
                    BindingFlags.Instance | BindingFlags.Public).GetValue(handler);
            }
        }
        
        public static Type GetDrawerTypeForType(Type type)
        {
            return (Type) _scriptAttributeUtilityType.GetMethod("GetDrawerTypeForType",
                BindingFlags.Static | BindingFlags.NonPublic).
                Invoke(null, new object[] { type });
        }

        public static bool HasVisibleChildFields(SerializedProperty property, bool isUIElements = false) =>
            (bool)_hasVisibleChildFieldsMethod.Invoke(null, new object[] { property, isUIElements });
    }
}
