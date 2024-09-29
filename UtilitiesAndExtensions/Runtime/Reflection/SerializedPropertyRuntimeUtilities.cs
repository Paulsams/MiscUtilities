using System;
using System.Collections;
using System.Reflection;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Auxiliary enam to separate parts of the array from the other.
    /// </summary>
    public enum SerializedPropertyFieldType
    {
        None,
        ArrayElement,
        ArraySize,
        Other,
    }

    /// <summary>
    /// Provides methods for using reflection to repeat certain logic of <see cref="T:UnityEditor.SerializedProperty"/> at runtime.
    /// </summary>
    public static class SerializedPropertyRuntimeUtilities
    {
        /// <summary>
        /// Gives a lot of information related to reflection based on <see cref="P:UnityEditor.SerializedProperty.propertyPath"/>.
        /// </summary>
        /// <param name="targetObject"> object from which the traversal will occur. </param>
        /// <param name="propertyPath"> path that should have semantics completely similar <see cref="P:UnityEditor.SerializedProperty.propertyPath"/>. </param>
        /// <returns> Tuple elements:
        /// <list type="bullet">
        /// <item><description> <see cref="T:System.Reflection.FieldInfo"/> at end object. </description></item>
        /// <item><description> <see cref="T:Paulsams.MicsUtils.SerializedPropertyFieldType"/> — additional information about property. </description></item>
        /// <item><description> Index in the array if <see cref="T:Paulsams.MicsUtils.SerializedPropertyFieldType"/> equals <see cref="Paulsams.MicsUtils.SerializedPropertyFieldType.ArrayElement"/>. </description></item>
        /// <item><description> Parent object. </description></item>
        /// <item><description> Object received from <paramref name="propertyPath"/> starting from <paramref name="targetObject"/>. </description></item>
        /// </list>
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException"></exception>
        public static (FieldInfo field, SerializedPropertyFieldType serializedPropertyFieldType,
            int? indexArrayElement, object parentObject, object currentObject) GetFieldInfoFromPropertyPath(
                object targetObject, string propertyPath)
        {
            FieldInfo GetFieldInfoForField(Type typeObject, string nameField)
            {
                var fieldInfo = typeObject.GetField(nameField,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo == null && typeObject.BaseType != null)
                    return GetFieldInfoForField(typeObject.BaseType, nameField);
                return fieldInfo;
            }

            var namesProperty = propertyPath.Split('.');
            object currentObject = targetObject;
            object parentObject = null;
            FieldInfo fieldInfo = null;
            int? indexArrayElement = null;
            SerializedPropertyFieldType serializedPropertyFieldType = SerializedPropertyFieldType.None;
            for (int i = 0; i < namesProperty.Length; ++i)
            {
                bool isArray = namesProperty[i] == "Array";
                if (isArray)
                {
                    // Method "Split" split the string "Array.data[xxx]" or "Array.size", so I do i + 1.
                    if (i + 1 < namesProperty.Length)
                    {
                        var array = currentObject as IList;
                        string propertyInArray = namesProperty[i + 1];
                        if (propertyInArray == "size")
                        {
                            currentObject = array.Count;
                            parentObject = array;
                            indexArrayElement = null;
                            serializedPropertyFieldType = SerializedPropertyFieldType.ArraySize;
                            ++i;
                            continue;
                        }
                        else if (propertyInArray.Contains("data["))
                        {
                            int indexInArray = GetIndexFromArrayProperty(propertyInArray);

                            if (array.Count <= indexInArray)
                                throw new InvalidOperationException(
                                    "Size of array is less than index found by propertyPath.");

                            currentObject = array[indexInArray];
                            parentObject = array;
                            indexArrayElement = indexInArray;
                            serializedPropertyFieldType = SerializedPropertyFieldType.ArrayElement;
                            ++i;
                            continue;
                        }
                    }
                }

                fieldInfo = GetFieldInfoForField(currentObject.GetType(), namesProperty[i]);
                parentObject = currentObject;
                currentObject = fieldInfo.GetValue(currentObject);
                indexArrayElement = null;
                serializedPropertyFieldType = SerializedPropertyFieldType.Other;
            }

            return (fieldInfo, serializedPropertyFieldType, indexArrayElement, parentObject, currentObject);
        }

        /// <summary>
        /// Getting index from <see cref="P:UnityEditor.SerializedProperty.propertyPath"/>, if <see cref="T:UnityEditor.SerializedProperty"/> along this path - it returns <see cref="P:UnityEditor.SerializedProperty.isArray"/> true..
        /// </summary>
        /// <param name="dataArray"> string representing <see cref="P:UnityEditor.SerializedProperty.propertyPath"/>. </param>
        public static int GetIndexFromArrayProperty(string dataArray)
        {
            int startIndex = dataArray.IndexOf('[') + 1;
            int endIndex = dataArray.Length - 1;

            string indexInArrayFromString = string.Empty;
            for (int j = startIndex; j < endIndex; ++j)
            {
                indexInArrayFromString += dataArray[j];
            }

            return int.Parse(indexInArrayFromString);
        }
    }
}