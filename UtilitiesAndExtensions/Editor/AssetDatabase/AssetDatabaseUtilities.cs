using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// Utilities associated with class: <see cref="T:UnityEditor.AssetDatabase"/>.
    /// </summary>
    public static class AssetDatabaseUtilities
    {
        /// <summary>
        /// A class that contains keys for searching using a method: <see cref="M:UnityEditor.AssetDatabase.FindAssets(System.String)"/>.
        /// </summary>
        public static class FilterKeys
        {
            public const string Scenes = "t:sceneasset";
            public const string Prefabs = "t:prefab";
            public const string ScriptableObjects = "t:ScriptableObject";
        }

        public static readonly string[] AssetsFolders = new string[] { "Assets" };

        /// <summary>
        /// Saves the current context of open scenes, lazily opens all scenes as they are fed out using an iterator,
        /// and restores the context at the end.
        /// </summary>
        /// <returns> Iterator all scenes
        /// (after all, to get GO from a scene, you need to open it, unfortunately). </returns>
        public static IEnumerable<Scene> GetAllScenesInAssets()
        {
            var oldScenesSetup = EditorSceneManager.GetSceneManagerSetup();

            (string scenePath, int identifierInFile)? oldSelectedObject = null;
            if (Selection.activeGameObject)
                oldSelectedObject = (
                    Selection.activeGameObject.scene.path,
                    Selection.activeGameObject.GetLocalIdentifierInFile()
                );

            foreach (var guid in AssetDatabase.FindAssets(FilterKeys.Scenes, AssetsFolders))
            {
                var scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(guid), OpenSceneMode.Single);
                yield return scene;
            }

            {
                EditorSceneManager.RestoreSceneManagerSetup(oldScenesSetup);

                if (oldSelectedObject.HasValue)
                {
                    Selection.activeGameObject = EditorSceneManager.GetSceneByPath(oldSelectedObject.Value.scenePath)
                        .GetRootGameObjects()
                        .FirstOrDefault(gameObject =>
                            gameObject.GetLocalIdentifierInFile() == oldSelectedObject.Value.identifierInFile);
                }
            }
        }
    }
}