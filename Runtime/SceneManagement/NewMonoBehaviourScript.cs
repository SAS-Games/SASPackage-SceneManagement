using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]

public class LevelStreamingData
{
    public string SceneName;
    public Bounds WorldBounds; // Axis-aligned bounding box of this level
    public int Priority;       // Higher = load earlier
    public bool AlwaysLoaded;  // Optional fallback for persistent sub-levels
}

public interface IStreamingSearchStrategy
{
    List<LevelStreamingData> Query(Bounds cameraBounds);
}

public interface IStreamingLoader
{
    void Load(LevelStreamingData level);
    void Unload(LevelStreamingData level);
}

public class LevelStreamingManager
{
    private readonly IStreamingSearchStrategy _searchStrategy;
    private readonly IStreamingLoader _loader;

    // Store metadata instead of just names
    private readonly Dictionary<string, LevelMetaData> _activeLevels = new();

    public LevelStreamingManager(
        IStreamingSearchStrategy searchStrategy,
        IStreamingLoader loader)
    {
        _searchStrategy = searchStrategy;
        _loader = loader;
    }

    public void Update(Bounds cameraBounds)
    {
        // Find candidate levels overlapping camera bounds
        var candidates = _searchStrategy.Query(cameraBounds).ToList();

        // Load new levels
        foreach (var level in candidates)
        {
            if (!_activeLevels.ContainsKey(level.SceneName))
            {
                _loader.Load(level);
                _activeLevels[level.SceneName] = level;
            }
        }

        // Unload levels no longer in range
        var toUnload = _activeLevels.Keys
            .Where(name => !candidates.Any(c => c.SceneName == name))
            .ToList();

        foreach (var sceneName in toUnload)
        {
            var level = _activeLevels[sceneName];
            _loader.Unload(level);
            _activeLevels.Remove(sceneName);
        }
    }
}