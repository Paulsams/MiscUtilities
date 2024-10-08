using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    [CustomPropertyDrawer(typeof(IgnoreAttribute))]
    public class IgnoreAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = 0f;
            foreach (var childProperty in property.GetChildren())
            {
                height += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            if (height > 0f)
                height -= EditorGUIUtility.standardVerticalSpacing;

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            foreach (var childProperty in property.GetChildren())
            {
                EditorGUI.PropertyField(position, childProperty, true);
                position.y += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}