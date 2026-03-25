using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "InkolorGames/Level Data")]
public class LevelData : ScriptableObject, IUnlockableItem
{
    [Scene, SerializeField] private string scene;
    public string Scene => scene;

    [SerializeField] private string itemId;
    [SerializeField] private string itemName;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public string ItemType => "Level";

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(itemId) && !string.IsNullOrEmpty(itemName))
        {
            itemId = itemName.Replace(" ", "_").ToLower();
        }
    }
}
