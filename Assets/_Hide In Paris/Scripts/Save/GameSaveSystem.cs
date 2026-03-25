using System.IO;
using inkolorgames;
using Newtonsoft.Json;
using UnityEngine;

public class GameSaveSystem : PersistentMonoSingleton<GameSaveSystem>
{
    [SerializeField] private inkolorgames.Logger logger;

    private GameSaveData gameSaveData;

    protected override void Awake()
    {
        base.Awake();
        LoadGameData();
    }

    private void LoadGameData()
    {
        string fullPath = SavePaths.FullPathSaveFile;

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            gameSaveData = JsonConvert.DeserializeObject<GameSaveData>(json);
        }
        else
        {
            gameSaveData ??= new GameSaveData();
        }
    }

    private void SaveGameData()
    {
        string fullPath = SavePaths.FullPathSaveFile;
        string json = JsonConvert.SerializeObject(gameSaveData, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        File.WriteAllText(fullPath, json);
    }

    public LevelSaveData LoadLevelData()
    {
        if (gameSaveData.levelSaveData == null)
            gameSaveData.levelSaveData = new LevelSaveData();

        return gameSaveData.levelSaveData;
    }

    public void SaveLevelData(LevelSaveData data)
    {
        gameSaveData.levelSaveData = data;
        SaveGameData();
        logger.Log("Level data saved", this);
    }

    public InsideLevelSaveData LoadInsideLevelData(string levelId) => gameSaveData.GetOrCreateInsideLevel(levelId);

    public void SaveInsideLevelData(InsideLevelSaveData data)
    {
        gameSaveData.SetInsideLevel(data);
        SaveGameData();
        logger.Log($"Inside level '{data.levelId}' saved.", this);
    }

    public void DeleteInsideLevelData(string levelId)
    {
        gameSaveData.insideLevels.RemoveAll(l => l.levelId == levelId);
        SaveGameData();
        logger.Log($"Inside level '{levelId}' save deleted.", this);
    }
}
