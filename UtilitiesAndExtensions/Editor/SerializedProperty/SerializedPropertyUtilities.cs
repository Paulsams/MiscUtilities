using System;
using System.Reflection;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities associated with class: <see cref="T:UnityEditor.SerializedProperty"/>.
    /// </summary>
    public static class SerializedPropertyUtilities
    {
        /// <summary>
        /// Gives <see cref="T:Type"/> to from <see cref="P:UnityEditor.SerializedProperty.managedReferenceFullTypename"/>.
        /// </summary>
        /// <param name="typename"> <see cref="P:UnityEditor.SerializedProperty.managedReferenceFullTypename"/> </param>
        public static Type GetManagedReferenceType(string typename)
        {
            var splitFieldTypename = typename.Split(' ');
            if (splitFieldTypename.Length != 2)
                return null;
            
            var assemblyName = splitFieldTypename[0];
            var typeName = splitFieldTypename[1];
            var assembly = Assembly.Load(assemblyName);
            var targetType = assembly.GetType(typeName);

            return targetType;
        }
    }
}
