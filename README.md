# MiscUtilities
Different utility classes for my other packages to work with.

## Add to project:

To add this package to the project, follow these steps:
1) Open PackageManager;
2) Select "Add package from get URL";
3) insert this link `https://github.com/Paulsams/MiscUtilities.git`

## Additional tips:
If you do not have a name conflict with my utilities, then you can make my namespace be added to the script automatically:
1) Create a folder "ScriptTemplates" in the root of the project;
2) Create a file with the name: "81-C# Script-NewBehaviourScript.cs";
3) And replace all the text in it with this text (one tab before #ROOTNAMESPACEBEGIN# is required):
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paulsams.MicsUtil;

    #ROOTNAMESPACEBEGIN#
public class #SCRIPTNAME# : MonoBehaviour
{
    
}
#ROOTNAMESPACEEND#
```

____

## Different Custom Editors:
  
1) GameObjectLayer is a structure that allows you to select a layer in the inspector without bit shift operators, that is, for comparison with GameObject.layer;
  
![image](https://github.com/Paulsams/MiscUtilities/blob/master/Documentation~/GameObjectLayer.gif)

2) ReadonlyField is an attribute so that you can see the value in the inspector, but not change it (but, as I understand it, it only works on non-custom types that do not have their own PropertyDrawer. And I think this because the custom PropertyDrawer redraws what ReadonlyAttribute called).
  
![image](https://github.com/Paulsams/MiscUtilities/blob/master/Documentation~/ReadonlyAttribute.gif)

## Extensions and Utilities:

### Editor:
1. MyEditorWindowUtility:
    + T GetWindowWithoutShow\<T>() where T : EditorWindow - allows you to get a EditorWindow without showing it.	
  
2. MyProjectWindowUtility:
    + bool TryGetActiveFolderPathInApplication(out string path) - allows you to get the absolute path to the currently open folder in the Project Window.
    + bool TryGetActiveFolderRelativePath(out string path) - allows you to get the relative path to the currently open folder in the Project Window.
  
3. SerializedPropertyExtensions:
    + object GetManagedReferenceValueFromPropertyPath(this SerializedProperty property) - allows you to get a managedReferenceValue that does not depend on the engine version.
  
    + object GetValueFromPropertyPath(this SerializedProperty property) - allows you to get an object through reflection on propertypath.
    + Type GetManagedReferenceFieldType(this SerializedProperty property) - allows you to get the Type of the object that currently lies in managedReferenceValue, that is, in an object with the [SerializeReference] attribute.
    + Type GetTypeObjectReference(this SerializedProperty property) - allows you to get the Type of field that is inherited from UnityEngine.Object.
    + IEnumerable\<SerializedProperty> GetChildrens(this SerializedProperty property) - allows you to get all the children from the current SerializedProperty.

4. SerializedPropertyUtilities:
    + int GetIndexFromArrayProperty(string dataArray) - allows you to get an index in the array for this type of string: "Array.data[xxx]".

### Runtime:
1. MyCompare - script for comparing strings and numbers based on the enumeration responsible for comparing objects.
  
2. DictionaryExtensions:
    + TKey KeyByValue\<TKey, TValue>(this IDictionary\<TKey, TValue> dictionary, TValue value) - allows you to get a key by value.
  
3. ListExtensions:
    + void Resize\<T>(this List<T> list, int needSize) - allows you to resize the list.
    + List\<T> Clone\<T>(this IList\<T> listToClone) where T : ICloneable - allows you to clone the list.
    + T FindMinDistanceElement\<T>(this IList\<T> list, Vector3 position, Func\<Component, Component, bool> additionally = null) where T : Component - allows you to find the closest GameObject by distance.
    + T FindMaxDistanceElement\<T>(this IReadOnlyList\<T> list, Vector3 position, Func\<Component, Component, bool> additionally = null) where T : Component - allows you to find the distant GameObject by distance.
  
4. JsonSerializerUtility - a class to simplify working with Newtonsoft JSON.
    + void Serialize\<T>(T serializedObject, string filePath, Formatting formatting = Formatting.Indented)
    + bool TryDeserialize\<T>(out T needObject, string filePath)
  
5. MathUtility:
    + float GetAngleFromVector(Vector2 direction) - allows you to get an angle based on the direction in the trigonometric system (0-360 degrees).
    + Vector2 GetVectorFromAngle(float angle) - allows you to get directions based on the angle in the trigonometric system (0-360 degrees).
    + float ClampAngle(float angle) - clamp angle in the range 0-360.
    + float NearestAngle(float angle, float stepRotation) - finds the nearest angle relative to the step.
    + float NearestAngle(float angle, float[] endAngles) - finds the nearest angle relative to the array of angles (-180-180).
  
6. OptimizedRect - an optimized replacement for Unity Engine.Rect.
  
7. PhysicsMyUtilities:
    + void IgnoreCollision(IList\<Collider> collidersFirst, IList\<Collider> collidersSecond, bool state) - ignoring collisions between arrays.
  
8. ReflectionExtensions:
    + T[] GetPublicConstFields\<T>(this Type type)
    + T[] GetPublicStaticReadonlyFields\<T>(this Type type)
  
9. ReflectionUtilities:
    + void CopyFieldsFromSourceToDestination(object source, object destination) - allows you to copy fields from completely different classes if they have the same Type and Name.
	+ Type GetArrayOrListElementTypeOrThisType(Type type) - allows you to find out the type of an array or sheet element, or returns the same type.
    + IEnumerable\<Type> GetAllTypesInCurrentDomain() - allows you to get a collection of all types in the current domain.
    + ReadOnlyCollection\<Type> GetFinalAssignableTypesFromAllTypes(Type baseType) - allows you to find out all Types that are inherited from a given Type and are not abstract or interfaces.
  
10. ExtensionsFromRuntimeCreateScript - an extensions class for creating scripts in runtime. NOTE: I don't recommend using it at all, because it's more convenient to do it through templates, but sometimes it happens that it's more convenient through this method:
    + void AppendLine(this StringBuilder stringBuilder, string text, int tabIndex)
    + void AppendOpeningBrace(this StringBuilder stringBuilder, ref int tabIndex)
    + void AppendBreakingBrace(this StringBuilder stringBuilder, ref int tabIndex, bool semicolon = false) - the "semilocon" argument is responsible for whether to put a semicolon after the breaking brace.
    + void AppendTabs(this StringBuilder stringBuilder, int tabIndex)
    + string ClearSpaces(this string text)
