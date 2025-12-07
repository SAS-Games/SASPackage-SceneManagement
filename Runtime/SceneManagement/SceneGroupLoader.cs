using SAS.SceneManagement;
using SAS.Utilities;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class SceneGroupLoader : Singleton<SceneGroupLoader>
{
    [Inject] private ISceneLoader _sceneLoader;
    protected override void Awake()
    {
        this.Initialize();
        base.Awake();
    }

    public void Load(string sceneGroupName, bool loadOptionalScenes = false, string setActiveScene = "Persistent", ILoadingScreen loadingScreen = null)
    {
        if (loadingScreen != null)
        {
            var screenMB = (loadingScreen as MonoBehaviour);
            var screenGO = screenMB.gameObject;

            Transform originalParent = screenGO.transform.parent;

            MoveToDontDestroy(screenGO);

            loadingScreen.OnFadeOutComplete += () =>
            {
                if (originalParent != null)
                    screenGO.transform.SetParent(originalParent, worldPositionStays: false);
                else if (screenGO != null)
                    Destroy(screenGO);
            };
        }

        _ = _sceneLoader.LoadSceneGroupAsync(sceneGroupName, loadOptionalScenes, setActiveScene, loadingScreen);
    }

    private void MoveToDontDestroy(GameObject obj)
    {
        if (obj == null)
            return;
        obj.transform.SetParent(null);
        DontDestroyOnLoad(obj);
    }
}