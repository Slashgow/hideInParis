using System;
using System.Collections.Generic;

[Serializable]
public class InsideLevelSaveData
{
    /// <summary>
    /// Matches the levelId set on HiddenObjectManager (defaults to scene name).
    /// </summary>
    public string levelId;

    /// <summary>
    /// Found / placed state for every HiddenObjectItem in this level.
    /// </summary>
    public List<HiddenObjectItemSaveData> hiddenObjectStates = new List<HiddenObjectItemSaveData>();

    public InsideLevelSaveData() { }

    public InsideLevelSaveData(string levelId)
    {
        this.levelId = levelId;
    }
}