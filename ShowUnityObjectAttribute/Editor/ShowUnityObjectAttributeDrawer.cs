using Paulsams.MicsUtils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.Search.ObjectField;

[CustomPropertyDrawer(typeof(ShowUnityObjectAttribute))]
public class ShowUnityObjectAttributeDrawer : PropertyDrawer
{
    private bool _showed;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();
        var label = property.displayName;

        Foldout foldout = new Foldout();
        foldout.value = _showed;
        foldout.text = label;
        VisualElementsUtilities.SetAlignedLabelFromFoldout(foldout, out VisualElement containerOnSameRowWithToggle,
            out VisualElement checkmark);
        
        void UpdateCheckmark() =>
            checkmark.style.visibility = property.objectReferenceValue
                ? Visibility.Visible
                : Visibility.Hidden;

        UpdateCheckmark();
        foldout.RegisterValueChangedCallback((callback) => _showed = callback.newValue);
        container.Add(foldout);
        
        var objectField = new ObjectField("");
        objectField.BindProperty(property);
        objectField.RegisterValueChangedCallback((_) => UpdateCheckmark());
        objectField.style.flexGrow = 1f;
        containerOnSameRowWithToggle.Add(objectField);

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
        if (_showed == false || property.objectReferenceValue == null)
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
        EditorGUI.ObjectField(objectRect, property, property.objectReferenceValue == null ? label : new GUIContent(""));

        if (property.objectReferenceValue == null)
            return;
        
        _showed = EditorGUI.Foldout(position, _showed, label, true);
        if (_showed)
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
    