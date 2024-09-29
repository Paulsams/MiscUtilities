using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities methods for different classes, but they all involve reflection.
    /// </summary>
    public static class ReflectionUtilities
    {
        /// <summary>
        /// Allows you to copy fields from completely different classes if they have the same <see cref="T:System.Type"/> and <see cref="P:System.Reflection.MemberInfo.Name"/>.
        /// <para> Works on the basis of reflection. </para>
        /// </summary>
        /// <param name="source"> object where field values are taken from. </param>
        /// <param name="destination"> object where values will be copied. </param>
        public static void CopyFieldsFromSourceToDestination(object source, object destination)
        {
            if (source == null || destination == null)
                return;

            var fieldsOldManagedReference = source.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var fieldsNewManagedReference = destination.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            for (int i = 0; i < fieldsNewManagedReference.Length; ++i)
            {
                var newField = fieldsNewManagedReference[i];
                var fieldNeedCopy = fieldsOldManagedReference.FirstOrDefault((oldField) =>
                    oldField.FieldType == newField.FieldType && oldField.Name == newField.Name);

                if (fieldNeedCopy != default)
                    newField.SetValue(destination, fieldNeedCopy.GetValue(source));
            }
        }

        /// <summary>
        /// It will return a new object depending on <paramref name="type"/> - if it has a constructor without arguments,
        /// it will create it through it, or through <see cref="System.Runtime.Serialization.FormatterServices.GetUninitializedObject(System.Type)"/>.
        /// </summary>
        public static object CreateObjectByDefaultConstructorOrUnitializedObject(Type type) =>
            type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null
                ? Activator.CreateInstance(type)
                : FormatterServices.GetUninitializedObject(type);

        /// <summary>
        /// If <paramref name="type"/> is an <see cref="T:System.Array"/> or <see cref="T:System.Collections.Generic.List`1"/>, it will return the type of the collection element, or return the same type.
        /// </summary>
        public static Type GetArrayOrListElementTypeOrThisType(Type type)
        {
            Type returnType = type;
            bool typeIsList = type.Name == "List`1" && type.Namespace == "System.Collections.Generic";

            if (typeIsList)
                returnType = type.GetGenericArguments()[0];
            else if (type.IsArray)
                returnType = type.GetElementType();

            return returnType;
        }

        /// <summary>
        /// Get collection of all types in the current domain.
        /// </summary>
        public static IEnumerable<Type> GetAllTypesInCurrentDomain() =>
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

        /// <summary>
        /// Find out all <see cref="T:System.Type"/>s that are inherited from a given <paramref name="baseType"/> and are not abstract or interfaces.
        /// </summary>
        /// <param name="baseType"> base type from which check will take place. </param>
        /// <returns></returns>
        public static IEnumerable<Type> GetFinalAssignableTypesFromAllTypes(Type baseType) =>
            GetAssignableTypesWhere(baseType, (type) => type.IsAbstract == false && type.IsInterface == false,
                GetAllTypesInCurrentDomain());

        private static IEnumerable<Type> GetAssignableTypesWhere(Type baseType,
            Predicate<Type> predicate, IEnumerable<Type> types) =>
            types.Where(type => baseType.IsAssignableFrom(type) && predicate(type));
    }
}