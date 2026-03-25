using System.IO;
using UnityEngine;

public static class SavePaths
{
    private static readonly string SAVE_FILE_NAME = "game_save.json";
    private static readonly string SAVE_FOLDER_BASE = Application.persistentDataPath;
    public static string SaveFolder => SAVE_FOLDER_BASE;
    public static string FullPathSaveFile => Path.Combine(SAVE_FOLDER_BASE, SAVE_FILE_NAME);
    public static string SaveFileName => SaveFileName;
}
