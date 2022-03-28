using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadonlyFieldAttribute))]
public class ReadonlyFieldAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var lastGUIState = GUI.enabled;
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = lastGUIState;
    }
}