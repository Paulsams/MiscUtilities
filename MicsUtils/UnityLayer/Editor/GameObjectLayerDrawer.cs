using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtil
{
    [CustomPropertyDrawer(typeof(GameObjectLayer))]
    public class GameObjectLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var layerProperty = property.FindPropertyRelative("_layer");

            layerProperty.intValue = EditorGUI.LayerField(position, layerProperty.intValue);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}