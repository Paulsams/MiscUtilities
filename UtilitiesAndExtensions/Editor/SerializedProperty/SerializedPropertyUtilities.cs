using System;
using System.Reflection;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class SerializedPropertyUtilities
    {
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
