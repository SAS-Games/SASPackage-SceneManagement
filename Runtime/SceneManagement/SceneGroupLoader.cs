using SAS.SceneManagement;
using SAS.Utilities.TagSystem;
using UnityEngine;

public class SceneGroupLoader : MonoBehaviour
{
    [Inject] private ISceneLoader _sceneLoader;
    [SerializeField] private string m_SceneGroupName;
    [SerializeField] private bool m_LoadOptionalScenes = false;
    [SerializeField] private bool m_LoadOnStart = false;

    [Tooltip("Before unloading the Current Scene Group, Set an active scene.")] [SerializeField]
    private string m_SetActiveScene = "Persistent";
    [SerializeField] private GameObject m_LoadingScreen;
    private ILoadingScreen _loadingScreen;

    private async void Start()
    {
        this.InjectFieldBindings();
        if (m_LoadingScreen)
        {
            _loadingScreen = m_LoadingScreen.GetComponentInChildren<ILoadingScreen>();
            if (_loadingScreen != null)
                _loadingScreen.OnFadeOutComplete += () => Destroy(gameObject);
        }

        if (m_LoadOnStart)
            await _sceneLoader.LoadSceneGroupAsync(m_SceneGroupName, m_LoadOptionalScenes, m_SetActiveScene, m_LoadingScreen);
    }

    public void Load()
    {
        _ = _sceneLoader.LoadSceneGroupAsync(m_SceneGroupName, m_LoadOptionalScenes, m_SetActiveScene, m_LoadingScreen);
    }
}