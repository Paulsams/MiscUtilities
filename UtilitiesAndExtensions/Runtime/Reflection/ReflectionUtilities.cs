using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Paulsams.MicsUtils
{
    public static class ReflectionUtilities
    {
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

        public static object CreateObjectByDefaultConstructorOrUnitializedObject(Type type) =>
            type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null
                ? Activator.CreateInstance(type)
                : FormatterServices.GetUninitializedObject(type);

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

        public static IEnumerable<Type> GetAllTypesInCurrentDomain() =>
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

        public static IEnumerable<Type> GetFinalAssignableTypesFromAllTypes(Type baseType) =>
            GetAssignableTypesWhere(baseType, (type) => type.IsAbstract == false && type.IsInterface == false,
                GetAllTypesInCurrentDomain());

        private static IEnumerable<Type> GetAssignableTypesWhere(Type baseType,
            Predicate<Type> predicate, IEnumerable<Type> types) =>
            types.Where(type => baseType.IsAssignableFrom(type) && predicate(type));
    }
}