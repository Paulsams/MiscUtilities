using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class UnityObjectExtensions
    {
        public static int GetLocalIdentifierInFile(this Object unityObject)
        {
            PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

            SerializedObject serializedObject = new SerializedObject(unityObject);
            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

            SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

            return localIdProp.intValue;
        }
    }
}
