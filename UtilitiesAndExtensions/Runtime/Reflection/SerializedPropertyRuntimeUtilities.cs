using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class SerializedPropertyRuntimeUtilities
    {
        public static (FieldInfo field, object parentObject, object currentObject) GetFieldInfoFromPropertyPath(object targetObject, string propertyPath)
        {
            FieldInfo GetFieldInfoForField(Type typeObject, string nameField)
            {
                var fieldInfo = typeObject.GetField(nameField, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (fieldInfo == null && typeObject.BaseType != null)
                    return GetFieldInfoForField(typeObject.BaseType, nameField);
                return fieldInfo;
            }

            var namesProperty = propertyPath.Split('.');
            object currentObject = targetObject;
            object parentObject = null;
            FieldInfo fieldInfo = null;
            for (int i = 0; i < namesProperty.Length; ++i)
            {
                //Method "Split" split the string "Array.data[xxx]", so I do i + 1.
                bool isArray = namesProperty[i] == "Array" && i + 1 < namesProperty.Length && namesProperty[i + 1].Contains("data[");
                if (isArray)
                {
                    string dataArray = namesProperty[i + 1];
                    int indexInArray = GetIndexFromArrayProperty(dataArray);

                    var array = currentObject as IList;

                    if (array.Count <= indexInArray)
                        throw new InvalidOperationException("Size of array is less than index found by propertyPath.");

                    currentObject = array[indexInArray];
                    ++i;
                    continue;
                }

                fieldInfo = GetFieldInfoForField(currentObject.GetType(), namesProperty[i]);
                parentObject = currentObject;
                currentObject = fieldInfo.GetValue(currentObject);
            }
            return (fieldInfo, parentObject, currentObject);
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
