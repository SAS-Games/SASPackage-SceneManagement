using Eflatun.SceneReference;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace SAS.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        [SerializeField] private string m_Name = "New Scene Group";
        public string Name => m_Name;

        [Tooltip("Scenes that are always loaded when this group loads")]
        [field: SerializeField]
        public List<SceneData> Scenes { get; private set; } = new List<SceneData>();

        [Tooltip("Scenes that can be always loaded or streamed based on metadata")]
        [field: SerializeField]
        public List<StreamingSceneData> StreamingScenes { get; private set; } = new List<StreamingSceneData>();

        public SceneData FindSceneDataByType(SceneType sceneType)
        {
            return Scenes.FirstOrDefault(scene => scene.SceneType == sceneType);
        }

        public string FindSceneNameByType(SceneType sceneType)
        {
            return FindSceneDataByType(sceneType)?.Reference.Name;
        }

        public Scene GetActiveScene()
        {
            return SceneManager.GetSceneByName(FindSceneNameByType(SceneType.ActiveScene));
        }

        public IEnumerable<SceneData> GetAlwaysLoadedStreamingScenes()
        {
            return StreamingScenes
                .Where(sub => sub.Mode == SubSceneLoadMode.Always)
                .Select(sub => sub.SceneData);
        }

        public IEnumerable<StreamingSceneData> GetDynamicStreamingScenes()
        {
            return StreamingScenes
                .Where(sub => sub.Mode == SubSceneLoadMode.Streaming);
        }
    }

    public enum SceneType
    {
        ActiveScene = 1,
        MainMenu,
        UserInterface,
        HUD,
        Cinematic,
        Environment,
    }

    [Serializable]
    public class SceneData
    {
        public SceneReference Reference;
        public string Name => Reference.Name;
        public SceneType SceneType;
        public bool IsOptional;
    }

    [Serializable]
    public class StreamingSceneData
    {
        public SceneData SceneData;
        public SubSceneLoadMode Mode;

        [Header("Streaming Metadata")] public int Priority = 0;
        public Bounds LoadBounds;
        public float UnloadDelay = 0f;
    }

    public enum SubSceneLoadMode
    {
        Always,
        Streaming
    }
}