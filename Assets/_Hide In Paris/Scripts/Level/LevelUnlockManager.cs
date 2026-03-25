using System;

public class LevelUnlockManager : UnlockManager<LevelData, LevelUnlockManager>
{
    private const string SAVE_KEY = "LEVEL";

    protected override string GetSaveKey() => SAVE_KEY;
    protected override UnlockableSaveData LoadSaveData()
    {
        LevelSaveData levelSaveData = GameSaveSystem.Instance.LoadLevelData();

        UnlockableSaveData saveData = new UnlockableSaveData();
        saveData.unlockedItemIds = new System.Collections.Generic.List<string>(levelSaveData.unlockedLevelIds);

        return saveData;
    }

    protected override void SaveData(UnlockableSaveData saveData)
    {
        LevelSaveData levelSaveData = new LevelSaveData();
        levelSaveData.unlockedLevelIds = new System.Collections.Generic.List<string>(saveData.unlockedItemIds);

        GameSaveSystem.Instance.SaveLevelData(levelSaveData);
    }
}
