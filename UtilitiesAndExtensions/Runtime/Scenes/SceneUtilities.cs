using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Scene-related utilities.
    /// </summary>
    public static class SceneUtilities
    {
        /// <summary>
        /// Get all components with <typeparamref name="T"/> type from current scenes.
        /// </summary>
        public static IEnumerable<T> GetAllComponentsInActiveScenes<T>() where T : Component =>
            Resources.FindObjectsOfTypeAll<T>()
                .Where((component) => string.IsNullOrEmpty(component.gameObject.scene.name) == false);

        /// <summary>
        /// Get all <see cref="T:UnityEngine.GameObject"/> from current scenes.
        /// </summary>
        public static IEnumerable<GameObject> GetAllGameObjectInActiveScenes() =>
            Resources.FindObjectsOfTypeAll<GameObject>()
                .Where((gameObject) => string.IsNullOrEmpty(gameObject.scene.name) == false);
    }
}