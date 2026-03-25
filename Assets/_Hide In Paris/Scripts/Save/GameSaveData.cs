using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameSaveData
{
    public LevelSaveData levelSaveData;
    public List<InsideLevelSaveData> insideLevels = new List<InsideLevelSaveData>();

    public GameSaveData()
    {
        levelSaveData = new LevelSaveData();
    }

    /// <summary>
    /// Returns the save data for the given level, creating a blank entry if
    /// this level has never been saved before.
    /// </summary>
    public InsideLevelSaveData GetOrCreateInsideLevel(string levelId)
    {
        var existing = insideLevels.FirstOrDefault(l => l.levelId == levelId);
        if (existing != null)
            return existing;

        var created = new InsideLevelSaveData(levelId);
        insideLevels.Add(created);
        return created;
    }

    /// <summary>
    /// Inserts or replaces the save data for a level.
    /// </summary>
    public void SetInsideLevel(InsideLevelSaveData data)
    {
        int index = insideLevels.FindIndex(l => l.levelId == data.levelId);
        if (index >= 0)
            insideLevels[index] = data;
        else
            insideLevels.Add(data);
    }
}