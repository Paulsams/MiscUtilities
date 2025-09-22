using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Paulsams.MicsUtils
{
    [CustomPropertyDrawer(typeof(ReadonlyFieldAttribute))]
    public class ReadonlyFieldAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var propertyField = new PropertyField();
            propertyField.BindProperty(property);
            propertyField.SetEnabled(false);
            return propertyField;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lastGUIState = GUI.enabled;
            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label);

            GUI.enabled = lastGUIState;
        }
    }
}