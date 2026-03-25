using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for managing unlocks of a specific item type
/// </summary>
/// <typeparam name="T">Type of item being managed</typeparam>
public interface IUnlockManager<T> where T : ScriptableObject, IUnlockableItem
{
    bool IsItemUnlocked(string itemId);
    void UnlockItem(string itemId);
    void LockItem(string itemId);
    T GetItemById(string itemId);
    List<Unlockable<T>> GetAllUnlockables();
    List<Unlockable<T>> GetLockedUnlockables();
    List<Unlockable<T>> GetUnlockedUnlockables();
}
