using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paulsams.MicsUtils
{
    [CustomPropertyDrawer(typeof(ShowUnityObjectAttribute))]
    public class ShowUnityObjectAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var label = property.displayName;

            Foldout foldout = new Foldout
            {
                value = property.isExpanded,
                text = label,
            };
            VisualElementsUtilities.SetAlignedLabelFromFoldout(foldout, out VisualElement containerOnSameRowWithToggle,
                out VisualElement checkmark);
        
            void UpdateCheckmark() =>
                checkmark.style.visibility = property.objectReferenceValue
                    ? Visibility.Visible
                    : Visibility.Hidden;

            UpdateCheckmark();
            foldout.RegisterValueChangedCallback(callback => property.isExpanded = callback.newValue);
            container.Add(foldout);

            var objectField = new ObjectField(null);
            objectField.objectType = property.GetTypeObjectReference();
            objectField.BindProperty(property);
            objectField.RegisterValueChangedCallback(_ => UpdateCheckmark());
            objectField.style.flexGrow = 1f;
            containerOnSameRowWithToggle.Add(objectField);
            
            if (property.objectReferenceValue == null)
                return container;

            var serializedObject = new SerializedObject(property.objectReferenceValue);
            
            var iterator = serializedObject.GetIterator();
            iterator.Next(true);
            while (iterator.NextVisible(false))
            {
                var propertyField = new PropertyField();
                propertyField.BindProperty(iterator);
                foldout.Add(propertyField);
            }

            return container;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float overallHeight = base.GetPropertyHeight(property, label);
            if (property.isExpanded == false || property.objectReferenceValue == null)
                return overallHeight;
        
            var serializedObject = new SerializedObject(property.objectReferenceValue);

            overallHeight += EditorGUIUtility.standardVerticalSpacing;

            var iterator = serializedObject.GetIterator();
            iterator.Next(true);
            while (iterator.NextVisible(false))
                overallHeight += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;

            return overallHeight - EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
            Rect objectRect = position;
            float offset = EditorGUIUtility.labelWidth + 2f;
            objectRect.x += offset;
            objectRect.width -= offset;
            EditorGUI.ObjectField(objectRect, property, property.objectReferenceValue == null
                ? label
                : new GUIContent(""));

            if (property.objectReferenceValue == null)
                return;
        
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                var serializedObject = new SerializedObject(property.objectReferenceValue);
        
                ++EditorGUI.indentLevel;
            
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            
                var iterator = serializedObject.GetIterator();
                iterator.Next(true);
                while (iterator.NextVisible(false))
                {
                    EditorGUI.PropertyField(position, iterator);
                    position.y += EditorGUI.GetPropertyHeight(iterator) + EditorGUIUtility.standardVerticalSpacing;
                }
            
                --EditorGUI.indentLevel;
            }
        }
    }
}
    