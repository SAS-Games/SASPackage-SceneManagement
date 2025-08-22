using SAS.Utilities.TagSystem;
using UnityEngine;

namespace SAS.SceneManagement
{
    public class GlobalLoadingScreenListener : MonoBehaviour
    {
        [FieldRequiresSelf] private ILoadingScreen _loadingScreenUI;

        private void Awake()
        {
            SceneLoaderExtensions.OnGlobalLoadingScreenRequested += HandleLoadingScreenRequest;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            SceneLoaderExtensions.OnGlobalLoadingScreenRequested -= HandleLoadingScreenRequest;
        }

        private void HandleLoadingScreenRequest(bool show)
        {
            if (_loadingScreenUI != null)
                _loadingScreenUI.SetActive(show);
        }
    }
}