using System;
using UnityEngine;

namespace inkolorgames
{
    public class DisplayManager : MonoSingleton<DisplayManager>
    {
        [SerializeField, Range(1, 240)] private int defaultFramerate = 60;
        [SerializeField] private bool defaultVSync = false;
        [SerializeField] private int defaultQuality = 0; // Default to lowest quality
        [SerializeField] private FullScreenMode defaultWindowMode = FullScreenMode.Windowed;

        public Vector2Int CurrentResolution { get; private set; }
        public int CurrentQuality { get; private set; }
        public bool CurrentVSync { get; private set; }
        public int CurrentFramerate { get; private set; }
        public FullScreenMode CurrentWindowMode { get; private set; }

        public readonly int[] framerateOptions = { 30, 60, 90, 120, 144, 240 };

        public const string RESOLUTION_WIDTH_ID = "ResolutionWidth";
        public const string RESOLUTION_HEIGHT_ID = "ResolutionHeight";
        public const string QUALITY_ID = "QualityLevel";
        public const string VSYNC_ID = "VSync";
        public const string FRAMERATE_ID = "Framerate";
        public const string WINDOW_MODE_ID = "WindowMode";

        public event Action OnResolutionChanged;

        protected override void Awake()
        {
            base.Awake();

            CurrentResolution = new Vector2Int(
                PlayerPrefs.GetInt(RESOLUTION_WIDTH_ID, Screen.currentResolution.width),
                PlayerPrefs.GetInt(RESOLUTION_HEIGHT_ID, Screen.currentResolution.height)
            );
            CurrentQuality = PlayerPrefs.GetInt(QUALITY_ID, defaultQuality);
            CurrentVSync = PlayerPrefs.GetInt(VSYNC_ID, defaultVSync ? 1 : 0) == 1;
            CurrentFramerate = PlayerPrefs.GetInt(FRAMERATE_ID, defaultFramerate);
            CurrentWindowMode = (FullScreenMode)PlayerPrefs.GetInt(WINDOW_MODE_ID, (int)defaultWindowMode);
        }

        private void Start()
        {
            ApplySettings();
        }

        public void SetResolution(Resolution resolution)
        {
            CurrentResolution = new Vector2Int(resolution.width, resolution.height);
            Screen.SetResolution(CurrentResolution.x, CurrentResolution.y, CurrentWindowMode);
            PlayerPrefs.SetInt(RESOLUTION_WIDTH_ID, CurrentResolution.x);
            PlayerPrefs.SetInt(RESOLUTION_HEIGHT_ID, CurrentResolution.y);
            OnResolutionChanged?.Invoke();
        }

        public void SetResolution(Vector2Int resolution)
        {
            CurrentResolution = resolution;
            Screen.SetResolution(CurrentResolution.x, CurrentResolution.y, CurrentWindowMode);
            PlayerPrefs.SetInt(RESOLUTION_WIDTH_ID, CurrentResolution.x);
            PlayerPrefs.SetInt(RESOLUTION_HEIGHT_ID, CurrentResolution.y);
            OnResolutionChanged?.Invoke();
        }

        public void SetQuality(int index)
        {
            CurrentQuality = index;
            QualitySettings.SetQualityLevel(CurrentQuality);
            PlayerPrefs.SetInt(QUALITY_ID, CurrentQuality);
        }

        public void SetVSync(bool value)
        {
            CurrentVSync = value;
            QualitySettings.vSyncCount = CurrentVSync ? 1 : 0;
            PlayerPrefs.SetInt(VSYNC_ID, CurrentVSync ? 1 : 0);
        }

        public void SetFramerate(int value)
        {
            CurrentFramerate = value;
            Application.targetFrameRate = CurrentFramerate;
            PlayerPrefs.SetInt(FRAMERATE_ID, CurrentFramerate);
        }

        public void SetWindowMode(FullScreenMode mode)
        {
            CurrentWindowMode = mode;
            Screen.fullScreenMode = CurrentWindowMode;
            PlayerPrefs.SetInt(WINDOW_MODE_ID, (int)CurrentWindowMode);
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(RESOLUTION_WIDTH_ID, CurrentResolution.x);
            PlayerPrefs.SetInt(RESOLUTION_HEIGHT_ID, CurrentResolution.y);
            PlayerPrefs.SetInt(QUALITY_ID, CurrentQuality);
            PlayerPrefs.SetInt(VSYNC_ID, CurrentVSync ? 1 : 0);
            PlayerPrefs.SetInt(FRAMERATE_ID, CurrentFramerate);
            PlayerPrefs.SetInt(WINDOW_MODE_ID, (int)CurrentWindowMode);
            PlayerPrefs.Save();
        }

        private void ApplySettings()
        {
            SetResolution(CurrentResolution); // Ensure current resolution is applied
            SetQuality(CurrentQuality);
            SetVSync(CurrentVSync);
            SetFramerate(CurrentFramerate);
            SetWindowMode(CurrentWindowMode);
        }
    }
}