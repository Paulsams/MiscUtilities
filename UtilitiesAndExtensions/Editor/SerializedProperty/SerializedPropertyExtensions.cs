using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;

namespace Paulsams.MicsUtils
{
    public static class SerializedPropertyExtensions
    {
        public static object GetManagedReferenceValueFromPropertyPath(this SerializedProperty property)
        {
            #if UNITY_2021_2_OR_NEWER
            object managedReferenceValue = property.managedReferenceValue;
            #else
            object managedReferenceValue = property.GetValueFromPropertyPath();
            #endif

            return managedReferenceValue;
        }

        public static object GetValueFromPropertyPath(this SerializedProperty property) => GetFieldInfoFromPropertyPath(property).currentObject;

        public static void SetValueFromPropertyPath(this SerializedProperty property, object value)
        {
            var fieldData = GetFieldInfoFromPropertyPath(property);
            switch (fieldData.serializedPropertyFieldType)
            {
                case SerializedPropertyFieldType.ArrayElement:
                    ((IList)fieldData.parentObject)[fieldData.indexArrayElement.Value] = value;
                    break;
                case SerializedPropertyFieldType.ArraySize:
                    throw new InvalidOperationException(
                        "Setting the size of the array through reflection is not supported yet.");
                default:
                    fieldData.field.SetValue(fieldData.parentObject, value);
                    break;
            }
        }

        public static (FieldInfo field, SerializedPropertyFieldType serializedPropertyFieldType,
            int? indexArrayElement, object parentObject, object currentObject) GetFieldInfoFromPropertyPath(this SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();

            return SerializedPropertyRuntimeUtilities.GetFieldInfoFromPropertyPath(property.serializedObject.targetObject, property.propertyPath);
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            var fieldTypename = property.managedReferenceFieldTypename;
            if (string.IsNullOrEmpty(fieldTypename))
                throw new Exception("ManagedReferenceFieldTypename is empty");
            return SerializedPropertyUtilities.GetManagedReferenceType(fieldTypename);
        }
        
        public static Type GetManagedReferenceFullType(this SerializedProperty property)
        {
            var fullTypename = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(fullTypename))
                throw new Exception("ManagedReferenceFieldTypename is empty");
            return SerializedPropertyUtilities.GetManagedReferenceType(fullTypename);
        }

        public static Type GetTypeObjectReference(this SerializedProperty property)
        {
            string nameTypeProperty = property.type.Substring(6).TrimEnd('>');
            foreach (Type type in ReflectionUtilities.GetAllTypesInCurrentDomain())
            {
                if (type.Name == nameTypeProperty)
                {
                    return type;
                }
            }

            throw new InvalidOperationException("The type of this SerializedProperty is not an inheritor of UnityEngine.Object.");
        }

        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
        {
            SerializedProperty currentProperty = property.Copy();
            SerializedProperty nextProperty = property.Copy();

            nextProperty.NextVisible(false);

            if (currentProperty.NextVisible(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.NextVisible(false));
            }
        }
        
        public static void CopyValueToOtherProperty(this SerializedProperty source, SerializedProperty destination,
            Func<SerializedProperty, SerializedProperty, bool> additionally = null)
        {
            if (source.type != destination.type &&
                (source.propertyType == SerializedPropertyType.ManagedReference &&
                source.managedReferenceFieldTypename != destination.managedReferenceFieldTypename &&
                ReflectionUtilities.GetFinalAssignableTypesFromAllTypes(destination.GetFieldInfoFromPropertyPath().field.FieldType).Contains(source.GetValueFromPropertyPath().GetType()) == false))
                throw new Exception($"{source.type} --- {destination.type}");

            Iterator(source, destination, additionally);
            destination.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Iterator(SerializedProperty source, SerializedProperty destination,
            Func<SerializedProperty, SerializedProperty, bool> overrideBehaviour)
        {
            if (overrideBehaviour != null)
            {
                if (overrideBehaviour.Invoke(source, destination))
                    return;
            }
            
            switch (source.propertyType)
            {
                case SerializedPropertyType.Generic:
                    var fieldData = source.GetFieldInfoFromPropertyPath();
                    var fieldType = fieldData.field.FieldType;
                    bool isArrayElement = fieldData.serializedPropertyFieldType ==
                                          SerializedPropertyFieldType.ArrayElement;
                    object newObj;
                    if (fieldType.IsArray && isArrayElement == false)
                    {
                        newObj = Array.CreateInstance(fieldType.GetElementType(), source.arraySize);
                    }
                    else if (fieldData.currentObject is IList list)
                    {
                        var newList = Activator.CreateInstance(fieldType) as IList;
                        for (int i = 0; i < list.Count; ++i)
                            newList.Add(list[i]);
                        
                        newObj = newList;
                    }
                    else
                    {
                        var objectType = isArrayElement
                            ? fieldType.IsArray
                                ? fieldType.GetElementType()
                                : fieldType.GetGenericArguments()[0]
                            : fieldType;
                        if (objectType.IsValueType || objectType.GetConstructor(Type.EmptyTypes) != null)
                            newObj = Activator.CreateInstance(objectType);
                        else
                            newObj = FormatterServices.GetUninitializedObject(objectType);
                    }

                    destination.SetValueFromPropertyPath(newObj);
                    break;
                case SerializedPropertyType.ManagedReference:
                    if (source.managedReferenceFullTypename == string.Empty)
                        break;

                    var assemblyNameAndTypeName = source.managedReferenceFullTypename.Split(' ');
                    var type = Assembly.Load(assemblyNameAndTypeName[0]).GetType(assemblyNameAndTypeName[1]);
                    //destination.managedReferenceValue = FormatterServices.GetUninitializedObject(type);
                    destination.managedReferenceValue = Activator.CreateInstance(type);
                    break;
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.ArraySize:
                    switch (source.numericType)
                    {
                        case SerializedPropertyNumericType.Int64:
                            destination.ulongValue = source.ulongValue;
                            break;
                        case SerializedPropertyNumericType.UInt64:
                            destination.longValue = source.longValue;
                            break;
                        case SerializedPropertyNumericType.UInt32:
                        case SerializedPropertyNumericType.UInt16:
                        case SerializedPropertyNumericType.UInt8:
                            destination.uintValue = destination.uintValue;
                            break;
                        case SerializedPropertyNumericType.Int16:
                        case SerializedPropertyNumericType.Int8:
                        default:
                            destination.intValue = source.intValue;
                            break;
                    }
                    break;
                case SerializedPropertyType.Boolean:
                    destination.boolValue = source.boolValue;
                    break;
                case SerializedPropertyType.Float:
                    if (source.numericType == SerializedPropertyNumericType.Double)
                        destination.doubleValue = source.doubleValue;
                    else
                        destination.floatValue = source.floatValue;
                    break;
                case SerializedPropertyType.String:
                    destination.stringValue = source.stringValue;
                    break;
                case SerializedPropertyType.Color:
                    destination.colorValue = source.colorValue;
                    break;
                case SerializedPropertyType.ObjectReference:
                    destination.objectReferenceInstanceIDValue = source.objectReferenceInstanceIDValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    destination.intValue = source.intValue;
                    break;
                case SerializedPropertyType.Vector2:
                    destination.vector2Value = source.vector2Value;
                    break;
                case SerializedPropertyType.Vector3:
                    destination.vector3Value = source.vector3Value;
                    break;
                case SerializedPropertyType.Vector4:
                    destination.vector4Value = source.vector4Value;
                    break;
                case SerializedPropertyType.Rect:
                    destination.rectValue = source.rectValue;
                    break;
                case SerializedPropertyType.Character:
                    destination.uintValue = source.uintValue;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    destination.animationCurveValue = source.animationCurveValue;
                    break;
                case SerializedPropertyType.Bounds:
                    destination.boundsValue = source.boundsValue;
                    break;
                case SerializedPropertyType.Gradient:
                    destination.gradientValue = source.gradientValue;
                    break;
                case SerializedPropertyType.Quaternion:
                    destination.quaternionValue = source.quaternionValue;
                    break;
                case SerializedPropertyType.ExposedReference:
                    destination.exposedReferenceValue = source.exposedReferenceValue;
                    break;
                case SerializedPropertyType.Vector2Int:
                    destination.vector2IntValue = source.vector2IntValue;
                    break;
                case SerializedPropertyType.Vector3Int:
                    destination.vector3IntValue = source.vector3IntValue;
                    break;
                case SerializedPropertyType.RectInt:
                    destination.rectIntValue = source.rectIntValue;
                    break;
                case SerializedPropertyType.BoundsInt:
                    destination.boundsIntValue = source.boundsIntValue;
                    break;
                case SerializedPropertyType.Hash128:
                    destination.hash128Value = source.hash128Value;
                    break;
                default:
                    throw new NotSupportedException($"Set on boxedValue property is not supported on \"{source.propertyPath}\" because it has an unsupported propertyType {source.propertyType}.");
            }

            if (source.propertyType == SerializedPropertyType.Generic || source.propertyType == SerializedPropertyType.ManagedReference)
            {
                var copiedDestination = destination.Copy();
                copiedDestination.NextVisible(true);
                foreach (var children in source.GetChildren())
                {
                    Iterator(children, copiedDestination, overrideBehaviour);
                    copiedDestination.NextVisible(false);
                }
            }
        }
    }
}
