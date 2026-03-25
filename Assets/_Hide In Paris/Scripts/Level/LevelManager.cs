using System;
using inkolorgames;
using UnityEngine;

public class LevelManager : PersistentMonoSingleton<LevelManager>
{
    public static event Action OnEndLevel;
    private LevelUnlockManager unlockManager;

    private void Start()
    {
        unlockManager = LevelUnlockManager.Instance;
    }
    private void OnEnable()
    {
        UILevel.OnTryStartLevel += UILevel_OnTryStartLevel;
    }

    private void OnDisable()
    {
        UILevel.OnTryStartLevel -= UILevel_OnTryStartLevel;
    }

    private void UILevel_OnTryStartLevel(LevelData levelData)
    {
        if (!unlockManager.IsItemUnlocked(levelData.ItemId))
        {
            Debug.LogWarning($"Brush '{levelData.ItemName}' is locked!");
            return;
        }

        SceneLoader.Instance.LoadSceneAsyncName(levelData.Scene);
    }
}
