using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic wrapper that makes any ScriptableObject unlockable
/// </summary>
/// <typeparam name="T">Type of item (must be ScriptableObject and IUnlockableItem)</typeparam>
[Serializable]
public class Unlockable<T> : IUnlockable where T : ScriptableObject, IUnlockableItem
{
    [SerializeField] private T item;
    [SerializeField] private List<UnlockCondition> unlockConditions = new List<UnlockCondition>();

    public T Item => item;
    public List<UnlockCondition> UnlockConditions => unlockConditions;

    private IUnlockManager<T> unlockManager;

    // Cached state to detect the locked→unlocked transition
    // so we only fire OnItemUnlocked once, and not on every CheckAllUnlocks call
    private bool _wasUnlockedLastCheck = false;
    private bool _initialized = false;

    public bool IsUnlocked => unlockManager != null && unlockManager.IsItemUnlocked(item.ItemId);

    public void Initialize(IUnlockManager<T> manager)
    {
        unlockManager = manager;
        // Do NOT call IsUnlocked here — GameSaveManager may not be ready yet.
        // _wasUnlockedLastCheck is seeded lazily on the first CheckUnlockCondition call.
    }

    public bool CheckUnlockCondition()
    {
        if (item == null)
        {
            Debug.LogWarning("Item is null in Unlockable!");
            return false;
        }

        // Lazy seed: first call after save system is ready
        if (!_initialized)
        {
            _wasUnlockedLastCheck = IsUnlocked;
            _initialized = true;
        }

        // Already unlocked in save — no need to re-evaluate conditions
        if (IsUnlocked)
        {
            _wasUnlockedLastCheck = true;
            return true;
        }

        // No conditions = unlocked by default
        if (unlockConditions == null || unlockConditions.Count == 0)
        {
            Unlock();
            return true;
        }

        bool allMet = true;
        foreach (var condition in unlockConditions)
        {
            if (!condition.IsMet())
            {
                allMet = false;
                break;
            }
        }

        // Only unlock (and fire the event) if this is a new transition
        if (allMet && !_wasUnlockedLastCheck)
        {
            Unlock();
        }

        _wasUnlockedLastCheck = allMet;
        return allMet;
    }

    public void Unlock()
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot unlock - Item is null!");
            return;
        }

        if (unlockManager == null)
        {
            Debug.LogWarning($"Cannot unlock - No unlock manager set for {item.ItemName}!");
            return;
        }

        if (IsUnlocked)
            return;

        unlockManager.UnlockItem(item.ItemId);
        _wasUnlockedLastCheck = true;
        Debug.Log($"{item.ItemType} '{item.ItemName}' unlocked!");
    }

    public void Lock()
    {
        if (item == null || unlockManager == null)
            return;

        unlockManager.LockItem(item.ItemId);
        _wasUnlockedLastCheck = false;
    }

    public string GetUnlockDescription()
    {
        if (unlockConditions == null || unlockConditions.Count == 0)
            return "Already unlocked";

        if (IsUnlocked)
            return "Unlocked";

        string description = "Unlock conditions:\n";
        for (int i = 0; i < unlockConditions.Count; i++)
        {
            description += $"- {unlockConditions[i].GetDescription()}";
            if (i < unlockConditions.Count - 1)
                description += "\n";
        }

        return description;
    }
}