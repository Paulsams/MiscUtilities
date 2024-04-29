using System;
using System.Collections;
using System.Reflection;

namespace Paulsams.MicsUtils
{
    public enum SerializedPropertyFieldType
    {
        None,
        ArrayElement,
        ArraySize,
        Other,
    }

    public static class SerializedPropertyRuntimeUtilities
    {
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