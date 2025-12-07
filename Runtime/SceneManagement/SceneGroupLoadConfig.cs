using SAS.SceneManagement;
using UnityEngine;

public class SceneGroupLoadConfig : MonoBehaviour
{
    public string SceneGroupName => m_SceneGroupName;
    public bool LoadOptionalScenes => m_LoadOptionalScenes;
    public string SetActiveScene => m_SetActiveScene;
    public GameObject LoadingScreen => m_LoadingScreen;

    [SerializeField] private string m_SceneGroupName;
    [SerializeField] private bool m_LoadOptionalScenes = false;
    [SerializeField] private bool m_LoadOnStart = false;

    [Tooltip("Before unloading the Current Scene Group, Set an active scene.")] [SerializeField]
    private string m_SetActiveScene = "Persistent";

    [SerializeField] private GameObject m_LoadingScreen;
    private ILoadingScreen _loadingScreen;

    private async void Start()
    {
        if (m_LoadOnStart)
            Load();
    }

    public void Load()
    {
        ILoadingScreen loadingScreen = null;
        if (_loadingScreen != null)
            loadingScreen = m_LoadingScreen.GetComponentInChildren<ILoadingScreen>();
        SceneGroupLoader.Instance.Load(m_SceneGroupName, m_LoadOptionalScenes, m_SetActiveScene, loadingScreen);
    }
}