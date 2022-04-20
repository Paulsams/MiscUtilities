using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

namespace Paulsams.MicsUtils
{
    public class MyProjectWindowUtility
    {
        public static bool TryGetActiveFolderAbsolutePath(out string path)
        {
            bool isFound = TryGetActiveFolderRelativePath(out path);
            if (isFound == false)
                return false;

            path = $"{Path.GetDirectoryName(Application.dataPath)}/{path}";

            return true;
        }

        public static bool TryGetActiveFolderRelativePath(out string path)
        {
            var tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);

            object[] arguments = new object[] { null };
            bool isFound = (bool)tryGetActiveFolderPath.Invoke(null, arguments);
            path = (string)arguments[0];

            return isFound;
        }
    }
}