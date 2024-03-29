using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class SceneUtilities
    {
        public static IEnumerable<T> GetAllComponentsInActiveScenes<T>() where T : Component
        {
            return Resources.FindObjectsOfTypeAll<T>().
                Where((component) => string.IsNullOrEmpty(component.gameObject.scene.name) == false);
        }

        public static IEnumerable<GameObject> GetAllGameObjectInActiveScenes()
        {
            return Resources.FindObjectsOfTypeAll<GameObject>().
                Where((gameObject) => string.IsNullOrEmpty(gameObject.scene.name) == false);
        }
    }
}
