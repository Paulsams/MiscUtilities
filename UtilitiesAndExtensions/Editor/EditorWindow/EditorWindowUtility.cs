using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities associated with class: <see cref="T:UnityEditor.EditorWindow"/>.
    /// </summary>
    public static class EditorWindowUtility
    {
        /// <summary>
        /// Get a window if it already exists or create a new one otherwise.
        /// <para> Can be useful if you want to get a window and do early initialization. </para>
        /// </summary>
        public static T GetWindowWithoutShow<T>() where T : EditorWindow
        {
            Object[] windows = Resources.FindObjectsOfTypeAll(typeof(T));
            return windows.Length != 0 ? (T)windows[0] : CreateWindow<T>();
        }

        /// <summary>
        /// Always creates a new <see cref="T:UnityEditor.EditorWindow"/>.
        /// </summary>
        public static T CreateWindow<T>() where T : EditorWindow => ScriptableObject.CreateInstance(typeof(T)) as T;
    }
}