using System.Collections.Generic;
/// <summary>
/// Interface for save data that tracks unlocked items
/// </summary>
public interface IUnlockableSaveData
{
    bool IsItemUnlocked(string itemId);
    void UnlockItem(string itemId);
    void LockItem(string itemId);
    List<string> GetUnlockedItemIds();
}
