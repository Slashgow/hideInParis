/// <summary>
/// Interface for any item that can be unlocked in the game
/// </summary>
public interface IUnlockableItem
{
    /// <summary>
    /// Unique identifier for this unlockable item
    /// </summary>
    string ItemId { get; }

    /// <summary>
    /// Display name for this item
    /// </summary>
    string ItemName { get; }

    /// <summary>
    /// Type category of this unlockable (e.g., "ColorTheme", "Brush", "Level")
    /// </summary>
    string ItemType { get; }
}
