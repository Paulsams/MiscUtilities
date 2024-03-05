using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Paulsams.MicsUtils
{
    public static class AssetDatabaseUtilities
    {
        public static IEnumerable<Scene> GetPathsToAllScenesInProject()
        {
            string activeScenePath = EditorSceneManager.GetActiveScene().path;
            
            foreach (var pathToScene in GetAssetsPathsFromFilenameExtension("unity"))
            {
                Scene scene = EditorSceneManager.OpenScene(pathToScene, OpenSceneMode.Single);
                yield return scene;
            }

            {
                EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
            }
        }

        public static IEnumerable<string> GetPathToAllPrefabsAssets()
        {
            return GetAssetsPathsFromFilenameExtension("prefab");
        }

        public static IEnumerable<string> GetAssetsPathsFromFilenameExtension(string filenameExtension)
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();

            foreach (string path in paths)
            {
                var lastDot = path.LastIndexOf('.');
                if (lastDot != -1)
                {
                    string currentExtension = path.Substring(lastDot + 1);
                    if (currentExtension == filenameExtension)
                        yield return path;
                }
            }
        }
    }
}