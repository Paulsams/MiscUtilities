using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Paulsams.MicsUtils
{
    public static class SerializedPropertyExtensions
    {
        public static object GetManagedReferenceValueFromPropertyPath(this SerializedProperty property)
        {
            #if UNITY_2021_2_OR_NEWER
            object managedReferenceValue = property.managedReferenceValue;
            #else
            object managedReferenceValue = property.GetValueFromPropertyPath();
            #endif

            return managedReferenceValue;
        }

        public static object GetValueFromPropertyPath(this SerializedProperty property) => GetFieldInfoFromPropertyPath(property).currentObject;

        public static void SetValueFromPropertyPath(this SerializedProperty property, object value)
        {
            var fieldData = GetFieldInfoFromPropertyPath(property);
            fieldData.field.SetValue(fieldData.parentObject, value);
        }

        public static (FieldInfo field, object parentObject, object currentObject) GetFieldInfoFromPropertyPath(this SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();

            return SerializedPropertyRuntimeUtilities.GetFieldInfoFromPropertyPath(property.serializedObject.targetObject, property.propertyPath);
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            var fieldTypeName = property.managedReferenceFieldTypename;
            if (string.IsNullOrEmpty(fieldTypeName))
                throw new Exception("ManagedReferenceFieldTypename is empty");

            var splitFieldTypename = fieldTypeName.Split(' ');
            var assemblyName = splitFieldTypename[0];
            var typeName = splitFieldTypename[1];
            var assembly = Assembly.Load(assemblyName);
            var targetType = assembly.GetType(typeName);

            return targetType;
        }

        public static Type GetTypeObjectReference(this SerializedProperty property)
        {
            string nameTypeProperty = property.type.Substring(6).TrimEnd('>');
            foreach (Type type in ReflectionUtilities.GetAllTypesInCurrentDomain())
            {
                if (type.Name == nameTypeProperty)
                {
                    return type;
                }
            }

            throw new InvalidOperationException("The type of this SerializedProperty is not an inheritor of UnityEngine.Object.");
        }

        public static IEnumerable<SerializedProperty> GetChildrens(this SerializedProperty property)
        {
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextProperty = property.Copy();

            nextProperty.NextVisible(false);

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
    }
}