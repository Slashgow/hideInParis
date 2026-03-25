using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace inkolorgames
{
    public class UIDisplay : MonoSingleton<UIDisplay>
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle vsyncToggle;
        [SerializeField] private TMP_Dropdown windowModeDropdown;

        private void Start()
        {
            PopulateDropdowns();
            LoadSettings();
        }

        private void PopulateDropdowns()
        {
            // Populate Resolution Dropdown
            resolutionDropdown.ClearOptions();
            var resolutions = Screen.resolutions.Select(r => $"{r.width}x{r.height} @{r.refreshRateRatio}Hz").ToList();
            resolutionDropdown.AddOptions(resolutions);

            // Populate Window Mode Dropdown
            windowModeDropdown.ClearOptions();
            windowModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Windowed", "Fullscreen" });
        }

        protected virtual void LoadSettings()
        {
            var currentResolution = Screen.resolutions.FirstOrDefault(r => r.width == DisplayManager.Instance.CurrentResolution.x && r.height == DisplayManager.Instance.CurrentResolution.y);
            resolutionDropdown.value = Screen.resolutions.ToList().IndexOf(currentResolution);
            vsyncToggle.isOn = DisplayManager.Instance.CurrentVSync;
            windowModeDropdown.value = DisplayManager.Instance.CurrentWindowMode == FullScreenMode.FullScreenWindow ? 1 : 0;
        }

        public void SetResolution(int index) => DisplayManager.Instance.SetResolution(Screen.resolutions[index]);
        public void SetVSync(bool value) => DisplayManager.Instance.SetVSync(value);
        public void SetWindowMode(int index) => DisplayManager.Instance.SetWindowMode(index == 1 ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }
}