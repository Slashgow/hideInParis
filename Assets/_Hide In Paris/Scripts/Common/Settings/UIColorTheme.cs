using System.Collections.Generic;
using inkolorgames;
using TMPro;
using UnityEngine;

public class UIColorTheme : MonoSingleton<UIColorTheme>
{
    [SerializeField] private TMP_Dropdown colorThemeDropdown;

    private void Start()
    {
        PopulateDropdown();
        LoadSettings();
    }

    private void PopulateDropdown()
    {
        colorThemeDropdown.ClearOptions();

        var colorThemes = new List<string>();
        foreach (var colorTheme in ColorManager.Instance.ThemeDatas)
        {
            colorThemes.Add(colorTheme.Name);
        }

        colorThemeDropdown.AddOptions(colorThemes);
    }

    private void LoadSettings()
    {
        int currentThemeIndex = ColorManager.Instance.CurrentColorThemeIndex;
        colorThemeDropdown.value = currentThemeIndex >= 0 ? currentThemeIndex : 0;
    }

    public void SetColorTheme(int index) => ColorManager.Instance.SetTheme(index);
}
