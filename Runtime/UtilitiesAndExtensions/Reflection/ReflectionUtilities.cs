using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Paulsams.MicsUtil
{
    public static class ReflectionUtilities
    {
        public static void CopyFieldsFromSourceToDestination(object source, object destination)
        {
            if (source != null)
            {
                var fieldsOldManagedReference = source.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                var fieldsNewManagedReference = destination.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                for (int i = 0; i < fieldsNewManagedReference.Length; ++i)
                {
                    var newField = fieldsNewManagedReference[i];
                    var fieldNeedCopy = fieldsOldManagedReference.FirstOrDefault((oldField) => oldField.FieldType == newField.FieldType && oldField.Name == newField.Name);

                    if (fieldNeedCopy != default)
                    {
                        newField.SetValue(destination, fieldNeedCopy.GetValue(source));
                    }
                }
            }
        }

        public static IEnumerable<Type> GetAllTypesInCurrentDomain() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes());

        public static ReadOnlyCollection<Type> GetFinalAssignableTypesFromAllTypes(Type baseType) => GetAssignableTypesWhere(baseType,
            (type) => type.IsAbstract == false && type.IsInterface == false, GetAllTypesInCurrentDomain());

        public static ReadOnlyCollection<Type> GetAssignableTypesWhere(Type baseType, Predicate<Type> predicate, IEnumerable<Type> types)
        {
            List<Type> assignableTypes = new List<Type>();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && predicate(type))
                {
                    assignableTypes.Add(type);
                }
            }
            return assignableTypes.AsReadOnly();
        }
    }
}