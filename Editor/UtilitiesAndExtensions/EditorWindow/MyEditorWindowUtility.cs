using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtil
{
    public static class MyEditorWindowUtility
    {
        public static T GetWindowWithoutShow<T>() where T : EditorWindow
        {
            Object[] windows = Resources.FindObjectsOfTypeAll(typeof(T));

            if (windows.Length != 0)
                return windows[0] as T;
            else
                return CreateWindow<T>();
        }

        public static T CreateWindow<T>() where T : EditorWindow => ScriptableObject.CreateInstance(typeof(T)) as T;
    }
}