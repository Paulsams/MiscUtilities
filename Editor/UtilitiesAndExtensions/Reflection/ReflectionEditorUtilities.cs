using System;
using System.Reflection;
using UnityEditor;

namespace Paulsams.MicsUtil
{
    public static class ReflectionEditorUtilities
    {
        public static Type GetFieldTypeFromSerializedProperty(SerializedProperty property)
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
    }
}