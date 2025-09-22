using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Extension methods for class: <see cref="T:UnityEditor.SerializedProperty"/>.
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Allows you to get a managedReferenceValue that does not depend on the engine version.
        /// </summary>
        /// <param name="property"> The property type is assumed to be <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>. </param>
        /// <returns></returns>
        public static object GetManagedReferenceValueFromPropertyPath(this SerializedProperty property)
        {
#if UNITY_2021_2_OR_NEWER
            object managedReferenceValue = property.managedReferenceValue;
#else
            object managedReferenceValue = property.GetValueFromPropertyPath();
#endif

            return managedReferenceValue;
        }

        /// <summary>
        /// Get an object through reflection on property path.
        /// Works like <see cref="P:UnityEditor.SerializedProperty.boxedValue"/> in new versions of the engine.
        /// </summary>
        /// <param name="property"> <see cref="T:UnityEditor.SerializedProperty"/> can be of any type. </param>
        /// <returns></returns>
        public static object GetValueFromPropertyPath(this SerializedProperty property) =>
            GetFieldInfoFromPropertyPath(property).currentObject;

        /// <summary>
        /// Sets the value using reflection on property path.
        /// Works like <see cref="P:UnityEditor.SerializedProperty.boxedValue"/> in new versions of the engine.
        /// </summary>
        /// <param name="property"> <see cref="T:UnityEditor.SerializedProperty"/> can be of any type,
        /// except <see cref="F:UnityEditor.SerializedPropertyType.ArraySize"/>.</param>
        /// <param name="value"> The object that will be set. </param>
        /// <exception cref="T:System.InvalidOperationException"> Does not support setting array size. </exception>
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

        /// <summary>
        /// Gives a lot of information related to reflection based on <see cref="T:UnityEditor.SerializedProperty"/>.
        /// </summary>
        /// <param name="property"> <see cref="T:UnityEditor.SerializedProperty"/> can be of any type </param>
        /// <returns> Tuple elements:
        /// <list type="bullet">
        /// <item><description> <see cref="T:System.Reflection.FieldInfo"/> at <see cref="T:UnityEditor.SerializedProperty"/>. </description></item>
        /// <item><description> <see cref="Paulsams.MicsUtils.SerializedPropertyFieldType"/> — additional information about property. </description></item>
        /// <item><description> Index in the array if <see cref="T:Paulsams.MicsUtils.SerializedPropertyFieldType"/> equals <see cref="Paulsams.MicsUtils.SerializedPropertyFieldType.ArrayElement"/>. </description></item>
        /// <item><description> Parent object. </description></item>
        /// <item><description> Object received from <see cref="T:UnityEditor.SerializedProperty"/>. </description></item>
        /// </list>
        /// </returns>
        public static (FieldInfo field, SerializedPropertyFieldType serializedPropertyFieldType,
            int? indexArrayElement, object parentObject, object currentObject) GetFieldInfoFromPropertyPath(
                this SerializedProperty property) =>
            SerializedPropertyRuntimeUtilities.GetFieldInfoFromPropertyPath(
                property.serializedObject.targetObject, property.propertyPath);

        /// <summary>
        /// Gives field <see cref="T:System.Type"/> to <see cref="T:UnityEditor.SerializedProperty"/> if the type is equal to <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>.
        /// </summary>
        /// <param name="property"> The property type is assumed to be <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>. </param>
        /// <exception cref="T:System.InvalidOperationException"> If the type <see cref="T:UnityEditor.SerializedProperty"/> is not equal to <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>. </exception>
        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            var fieldTypename = property.managedReferenceFieldTypename;
            if (string.IsNullOrEmpty(fieldTypename))
                throw new InvalidOperationException("ManagedReferenceFieldTypename is empty");
            return SerializedPropertyUtilities.GetManagedReferenceType(fieldTypename);
        }

        /// <summary>
        /// Gives this object <see cref="T:System.Type"/> to <see cref="T:UnityEditor.SerializedProperty"/> if the type is equal to <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>.
        /// </summary>
        /// <param name="property"> The property type is assumed to be <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>. </param>
        /// <exception cref="T:System.InvalidOperationException"> If the type <see cref="T:UnityEditor.SerializedProperty"/> is not equal to <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>. </exception>
        public static Type GetManagedReferenceFullType(this SerializedProperty property)
        {
            var fullTypename = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(fullTypename))
                throw new InvalidOperationException("ManagedReferenceFieldTypename is empty");
            return SerializedPropertyUtilities.GetManagedReferenceType(fullTypename);
        }

        /// <summary>
        /// Gives UnityEgine.Object <see cref="T:System.Type"/> to <see cref="T:UnityEditor.SerializedProperty"/> if the type is equal to <see cref="F:UnityEditor.SerializedPropertyType.ObjectReference"/>.
        /// </summary>
        /// <param name="property"> The property type is assumed to be <see cref="F:UnityEditor.SerializedPropertyType.ObjectReference"/>. </param>
        /// <exception cref="T:System.InvalidOperationException"> If the type <see cref="T:UnityEditor.SerializedProperty"/> is not equal to <see cref="F:UnityEditor.SerializedPropertyType.ObjectReference"/>. </exception>
        public static Type GetTypeObjectReference(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
                throw new InvalidOperationException(
                    "The type of this SerializedProperty is not an inheritor of UnityEngine.Object.");

            string nameTypeProperty = property.type.Substring(6).TrimEnd('>');
            var outputType = TypeCache
                .GetTypesDerivedFrom<UnityEngine.Object>()
                .FirstOrDefault(type => type.Name == nameTypeProperty);

            if (outputType == null)
                throw new InvalidOperationException("Couldn't find type");

            return outputType;
        }

        /// <summary>
        /// Getting all the children <see cref="T:UnityEditor.SerializedProperty"/>.
        /// </summary>
        /// <param name="property"> Родительский <see cref="T:UnityEditor.SerializedProperty"/>. </param>
        /// <returns> Iterator returning children. </returns>
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
                } while (currentProperty.NextVisible(false));
            }
        }

        /// <summary>
        /// Получение родительского <see cref="T:UnityEditor.SerializedProperty"/>.
        /// <para>IMPORTNANT: will not work if <see cref="T:UnityEditor.SerializedProperty"/> - it returns <see cref="P:UnityEditor.SerializedProperty.isArray"/> true. </para>
        /// </summary>
        /// <param name="property"> <see cref="T:UnityEditor.SerializedProperty"/> from which the parent is taken. </param>
        /// <returns> Parent <see cref="T:UnityEditor.SerializedProperty"/>. </returns>
        public static SerializedProperty GetParentProperty(this SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ArraySize)
            {
                return property.serializedObject.FindProperty(property.propertyPath
                    .Remove(property.propertyPath.Length - ".Array.size".Length));
            }
            if (property.propertyPath[property.propertyPath.Length - 1] == ']')
            {
                return property.serializedObject.FindProperty(property.propertyPath
                    .Remove(property.propertyPath.LastIndexOf(".Array.data[")));
            }
            int dotIndex = property.propertyPath.Length - property.name.Length - 1;
            if (dotIndex < 0)
                return null;

            var parentPropertyPath = property.propertyPath.Remove(dotIndex);
            return property.serializedObject.FindProperty(parentPropertyPath);
        }

        /// <summary>
        /// Copies all data from one <see cref="T:UnityEditor.SerializedProperty"/> to another <see cref="T:UnityEditor.SerializedProperty"/>.
        /// </summary>
        /// <param name="source"> Where values come from. </param>
        /// <param name="destination"> Where values are copied. </param>
        /// <param name="applyWithIsUndo"> Применять ли <see cref="M:UnityEditor.SerializedObject.ApplyModifiedProperties"/> или <see cref="M:UnityEditor.SerializedObject.ApplyModifiedPropertiesWithoutUndo"/> </param>
        /// <param name="additionally"> An optional delegate that sorts the copy value. If it returns false, then iteration through children does not occur either. </param>
        /// <exception cref="T:System.InvalidOperationException"> May be thrown out if two properties cannot be copied, if for
        /// <see cref="P:UnityEditor.SerializedProperty.propertyType"/> both properties have a value equal to <see cref="F:UnityEditor.SerializedPropertyType.ManagedReference"/>
        /// and do not have a common base type. </exception>
        public static void CopyValueToOtherProperty(this SerializedProperty source, SerializedProperty destination,
            bool applyWithIsUndo, Func<SerializedProperty, SerializedProperty, bool> additionally = null)
        {
            if (source.type != destination.type &&
                source.propertyType == SerializedPropertyType.ManagedReference &&
                source.managedReferenceFieldTypename != destination.managedReferenceFieldTypename &&
                TypeCache.GetTypesDerivedFrom(destination.GetFieldInfoFromPropertyPath().field.FieldType)
                    .Contains(source.GetValueFromPropertyPath().GetType()) == false
               )
                throw new InvalidOperationException($"{source.type} --- {destination.type}");

            Iterator(source, destination, applyWithIsUndo, additionally);
            if (applyWithIsUndo)
                destination.serializedObject.ApplyModifiedProperties();
            else
                destination.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void Iterator(SerializedProperty source, SerializedProperty destination,
            bool applyWithIsUndo, Func<SerializedProperty, SerializedProperty, bool> overrideBehaviour)
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
                        newObj = ReflectionUtilities.CreateObjectByDefaultConstructorOrUnitializedObject(objectType);
                    }

                    if (applyWithIsUndo)
                        destination.serializedObject.ApplyModifiedProperties();
                    else
                        destination.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    destination.SetValueFromPropertyPath(newObj);
                    break;
                case SerializedPropertyType.ManagedReference:
                    if (source.managedReferenceFullTypename == string.Empty)
                    {
                        destination.managedReferenceValue = null;
                        break;
                    }

                    var assemblyNameAndTypeName = source.managedReferenceFullTypename.Split(' ');
                    var type = Assembly.Load(assemblyNameAndTypeName[0]).GetType(assemblyNameAndTypeName[1]);
                    destination.managedReferenceValue =
                        ReflectionUtilities.CreateObjectByDefaultConstructorOrUnitializedObject(type);
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
                    throw new NotSupportedException(
                        $"Set on boxedValue property is not supported on \"{source.propertyPath}\"" +
                        $"because it has an unsupported propertyType {source.propertyType}.");
            }

            if (source.propertyType == SerializedPropertyType.Generic ||
                source.propertyType == SerializedPropertyType.ManagedReference)
            {
                var copiedDestination = destination.Copy();
                copiedDestination.NextVisible(true);
                foreach (var children in source.GetChildren())
                {
                    Iterator(children, copiedDestination, applyWithIsUndo, overrideBehaviour);
                    copiedDestination.NextVisible(false);
                }
            }
        }
    }
}