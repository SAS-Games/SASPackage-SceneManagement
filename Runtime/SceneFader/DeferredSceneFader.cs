using System;
using SAS.SceneManagement;
using SAS.Core.TagSystem;
using UnityEngine;

public class DeferredSceneFader : SceneFader
{
    [SerializeField] private string m_SceneName;

    protected override void Awake()
    {
        base.Awake();
        this.Initialize();
    }
    public override async void SetActive(bool active)
    {
        if (active)
            base.SetActive(active);
        else
        {
            var readyDependencyGroup = SceneUtility.FindComponentInScene<ReadyDependencyGroup>(m_SceneName);
            if (readyDependencyGroup != null)
            {
                try
                {
                    await readyDependencyGroup.WaitUntilReadyAsync();
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            if (this == null)
                return;

            base.SetActive(false);
        }
    }
}
