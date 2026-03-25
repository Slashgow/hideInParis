using System;
using System.Collections.Generic;

[Serializable]
public class UnlockableSaveData : IUnlockableSaveData
{
    public List<string> unlockedItemIds = new List<string>();

    public UnlockableSaveData()
    {
        unlockedItemIds = new List<string>();
    }

    public bool IsItemUnlocked(string itemId)
    {
        return unlockedItemIds.Contains(itemId);
    }

    public void UnlockItem(string itemId)
    {
        if (!unlockedItemIds.Contains(itemId))
        {
            unlockedItemIds.Add(itemId);
        }
    }

    public void LockItem(string itemId)
    {
        unlockedItemIds.Remove(itemId);
    }

    public List<string> GetUnlockedItemIds()
    {
        return new List<string>(unlockedItemIds);
    }
}