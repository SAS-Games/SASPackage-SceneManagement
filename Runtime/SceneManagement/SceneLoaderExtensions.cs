using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SAS.SceneManagement
{
    public static class SceneLoaderExtensions
    {
        /// <summary>
        /// Event fired when a scene group load starts/ends but no local loading screen is provided.
        /// </summary>
        public static event Action<bool> OnGlobalLoadingScreenRequested;
        // true = show, false = hide

        /// <summary>
        /// Loads a scene group with optional loading screen and moves the given GameObject
        /// to the target active scene before unloading the current group.
        /// </summary>
        public static async Task LoadSceneGroupAsync(
            this ISceneLoader sceneLoader,
            string sceneGroupName,
            bool loadOptionalScenes = false,
            string setActiveScene = "Persistent",
            GameObject loadingScreen = null)
        {
            if (loadingScreen != null)
                loadingScreen.SetActive(true);
            else
                OnGlobalLoadingScreenRequested?.Invoke(true);

            if (string.IsNullOrEmpty(sceneGroupName))
                sceneGroupName = sceneLoader.SceneGroupModel.ActiveSceneGroup.Name;

            if (!string.IsNullOrEmpty(setActiveScene))
            {
                var scene = SceneUtility.GetScene(setActiveScene);
                if (scene.isLoaded && loadingScreen != null)
                {
                    SceneUtility.MoveGameObjectToScene(loadingScreen.transform.root.gameObject, scene);
                    SceneUtility.SetActiveScene(setActiveScene);
                }
            }
            else
            {
                var scene = SceneUtility.GetScene("Bootstrapper");
                if (loadingScreen != null)
                    SceneUtility.MoveGameObjectToScene(loadingScreen.transform.root.gameObject, scene);
            }

            await sceneLoader.LoadSceneGroup(sceneGroupName, !loadOptionalScenes);

            Debug.Log($"Scene Group: {sceneGroupName} is loaded. Hide loading screen");

            if (loadingScreen != null)
                loadingScreen.SetActive(false);
            else
                OnGlobalLoadingScreenRequested?.Invoke(false);
        }
    }
}