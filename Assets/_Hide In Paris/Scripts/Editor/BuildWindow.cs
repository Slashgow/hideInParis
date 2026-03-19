using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace inkolorgames
{
    public class BuildWindow : EditorWindow
    {
        string majorString;
        string minorString;
        string patchString;
        private int major, minor, patch;
        private string buildPath;
        private bool copySteamAppId = true;

        private const string BUILD_PATH_KEY = "BUILD_PATH";

        [MenuItem("Tools/InkolorGames/Build Version Parameter")]
        public static void ShowWindow()
        {
            GetWindow<BuildWindow>("Build Version Parameter");
        }

        private void OnEnable()
        {
            string[] splitBundleVersion = PlayerSettings.bundleVersion.Split('[', '.', '.', '.', ']');
            if (splitBundleVersion.Length >= 2)
                majorString = splitBundleVersion[1];
            if (splitBundleVersion.Length >= 3)
                minorString = splitBundleVersion[2];
            if (splitBundleVersion.Length >= 4)
                patchString = splitBundleVersion[3];

            if (!int.TryParse(majorString, out major))
            {
                major = 0;
                majorString = major.ToString();
                UpdateVersion($"{0}.{0}.{0}");
            }
            if (!int.TryParse(minorString, out minor))
            {
                minor = 0;
                minorString = minor.ToString();
                UpdateVersion($"{0}.{0}.{0}");
            }
            if (!int.TryParse(patchString, out patch))
            {
                patch = 0;
                patchString = patch.ToString();
                UpdateVersion($"{0}.{0}.{0}");
            }

            buildPath = PlayerPrefs.GetString(BUILD_PATH_KEY);
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Major {majorString}", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
            {
                major++;
                majorString = major.ToString();
                UpdateVersion($"{major}.{minor}.{patch}");
            }
            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
            {
                major--;
                majorString = major.ToString();
                UpdateVersion($"{major}.{minor}.{patch}");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Minor {minorString}", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
            {
                minor++;
                minorString = minor.ToString();
                UpdateVersion($"{major}.{minor}.{patch}");
            }
            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
            {
                minor--;
                minorString = minor.ToString();
                UpdateVersion($"{major}.{minor}.{patch}");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Patch {patchString}", GUILayout.ExpandWidth(false));
            if (GUILayout.Button("+", GUILayout.Width(30), GUILayout.Height(30)))
            {
                patch++;
                patchString = patch.ToString();
                UpdateVersion($"{major}.{minor}.{patch}");
            }
            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(30)))
            {
                patch--;
                patchString = patch.ToString();
                UpdateVersion($"{major}.{minor}.{patch}");
            }
            EditorGUILayout.EndHorizontal();

            copySteamAppId = GUILayout.Toggle(copySteamAppId, "Copy Steam App ID");

            if (GUILayout.Button("Build"))
                Build();
        }


        private void Build()
        {
            BuildPlayerOptions defaultOptions = new BuildPlayerOptions();
            BuildPlayerOptions currentBuildOption = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(defaultOptions);
            buildPath = currentBuildOption.locationPathName.Split('\\')[0];
            Debug.Log($"saved build path {buildPath}");

            PlayerPrefs.SetString(BUILD_PATH_KEY, buildPath);
            buildPath = $"{buildPath}/{Application.productName}_{major}.{minor}.{patch}";
            if (!Directory.Exists(buildPath))
                Directory.CreateDirectory(buildPath);
            currentBuildOption.locationPathName = $"{buildPath}/{PlayerSettings.productName}.exe";
            Debug.Log(currentBuildOption.locationPathName);

            BuildPipeline.BuildPlayer(currentBuildOption);

            if (copySteamAppId)
                FileUtil.CopyFileOrDirectory($"{Application.dataPath.Split("/Assets")[0]}/steam_appid.txt", buildPath + "/steam_appid.txt");

        }


        private void UpdateVersion(string version)
        {
            string date = DateTime.Now.ToString("d");
            PlayerSettings.bundleVersion = string.Format("v [{0}] - {1} ", version, date);
            Debug.Log(PlayerSettings.bundleVersion);
        }
    }
}