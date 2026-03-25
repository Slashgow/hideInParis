using System;

[Serializable]
public class HiddenObjectItemSaveData
{
    /// <summary>
    /// Stable scene identifier: "{groupId}/{gameObject.name}"
    /// Must be unique per level — avoid duplicate GameObject names within the same group.
    /// </summary>
    public string sceneKey;

    public bool found;
    public bool placed;

    public HiddenObjectItemSaveData() { }

    public HiddenObjectItemSaveData(string sceneKey, bool found, bool placed)
    {
        this.sceneKey = sceneKey;
        this.found = found;
        this.placed = placed;
    }
}