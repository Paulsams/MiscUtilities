using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Paulsams.MicsUtil
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

        public static object GetValueFromPropertyPath(this SerializedProperty property)
        {
            object GetValueForField(object current, string nameField) =>
                current.GetType().GetField(nameField, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetValue(current);

            property.serializedObject.ApplyModifiedProperties();

            var namesProperty = property.propertyPath.Split('.');
            object currentObject = property.serializedObject.targetObject;
            for (int i = 0; i < namesProperty.Length; ++i)
            {
                //Method "Split" split the string "Array.data[xxx]", so I do i + 1.
                bool isArray = namesProperty[i] == "Array" && i + 1 < namesProperty.Length && namesProperty[i + 1].Contains("data[");
                if (isArray)
                {
                    string dataArray = namesProperty[i + 1];
                    int indexInArray = SerializedPropertyUtilities.GetIndexFromArrayProperty(dataArray);

                    var array = currentObject as IList;

                    if (array.Count <= indexInArray)
                        throw new InvalidOperationException("Size of array is less than index found by propertyPath.");

                    currentObject = array[indexInArray];
                    ++i;
                    continue;
                }

                currentObject = GetValueForField(currentObject, namesProperty[i]);
            }
            return currentObject;
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