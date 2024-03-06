using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Paulsams.MicsUtils
{
    public static class AssetDatabaseUtilities
    {
        public static IEnumerable<Scene> GetPathsToAllScenesInProject()
        {
            var oldScenesSetup = EditorSceneManager.GetSceneManagerSetup();

            (string scenePath, int identifierInFile)? oldSelectedObject = null;
            if (Selection.activeGameObject)
                oldSelectedObject = (
                    Selection.activeGameObject.scene.path,
                    Selection.activeGameObject.GetLocalIdentifierInFile()
                );

            foreach (var pathToScene in GetAssetsPathsFromFilenameExtension("unity"))
            {
                Scene scene = EditorSceneManager.OpenScene(pathToScene, OpenSceneMode.Single);
                yield return scene;
            }

            {
                EditorSceneManager.RestoreSceneManagerSetup(oldScenesSetup);

                Selection.activeGameObject = oldSelectedObject.HasValue
                    ? EditorSceneManager.GetSceneByPath(oldSelectedObject.Value.scenePath)
                        .GetRootGameObjects()
                        .FirstOrDefault(gameObject =>
                            gameObject.GetLocalIdentifierInFile() == oldSelectedObject.Value.identifierInFile)
                    : null;
            }
        }

        public static IEnumerable<string> GetPathToAllPrefabsAssets() =>
            GetAssetsPathsFromFilenameExtension("prefab");

        public static IEnumerable<string> GetAssetsPathsFromFilenameExtension(string filenameExtension)
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();

            foreach (string path in paths)
                if (path.EndsWith(filenameExtension))
                    yield return path;
        }
    }
}