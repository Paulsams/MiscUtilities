using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paulsams.MicsUtils
{
    [CustomPropertyDrawer(typeof(GameObjectLayer))]
    public class GameObjectLayerDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var layerProperty = property.FindPropertyRelative("_layer");
            var layerField = new LayerField(layerProperty.displayName);
            var label = layerField.Q<Label>();
            label.style.width = EditorGUIUtility.labelWidth;
            label.AddToClassList(PropertyField.labelUssClassName);
            layerField.BindProperty(layerProperty);
            return layerField;
        }

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