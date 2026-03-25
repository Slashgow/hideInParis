using System;
using System.Collections.Generic;
using System.Linq;
using inkolorgames;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static ColorManager;

public class ColorManager : MonoSingleton<ColorManager>
{
    public enum ColorTheme
    {
        BEIGE_AND_BLUE,
        BLACK_AND_WHITE,
        CYAN_AND_ROSE,
        BEIGE_AND_ROSE,
        MAUVE_AND_BLUE
    }

    [Serializable]
    public class ColorThemeData
    {
        [SerializeField] private ColorTheme theme;
        [SerializeField] private string name;
        [SerializeField] private Color backgroundColor;
        [SerializeField] private Color outlineColor;

        public ColorTheme Theme => theme;
        public string Name => name;
        public Color BackgroundColor => backgroundColor;
        public Color OutlineColor => outlineColor;
    }


    [SerializeField] private List<ColorThemeData> themeDatas;
    [SerializeField] private BlackReplacementFeature colorReplacementFeature;
    
    private Volume volume;
    private ColorAdjustments colorAdjustments;

    public List<ColorThemeData> ThemeDatas => themeDatas;

    public ColorTheme CurrentColorTheme { get; private set; }
    public int CurrentColorThemeIndex => themeDatas.IndexOf(GetThemeDataByTheme(CurrentColorTheme));

    public const string COLOR_THEME_ID = "COLOR_THEME";

    protected override void Awake()
    {
        base.Awake();

        volume = FindAnyObjectByType<Volume>();

        if (volume == null)
            return;

        if(volume.profile.TryGet(out ColorAdjustments colorAdjustments))
            this.colorAdjustments = colorAdjustments;

        if (PlayerPrefs.HasKey(COLOR_THEME_ID))
        {
            int index = PlayerPrefs.GetInt(COLOR_THEME_ID);
            ColorThemeData colorThemeData = GetThemeDataByIndex(index);
            SetTheme(colorThemeData.Theme);
        }
        else
            SetTheme(ColorTheme.BEIGE_AND_BLUE);
    }

    private int GetIndexByTheme(ColorTheme theme) => themeDatas.IndexOf(GetThemeDataByTheme(theme));
    private ColorThemeData GetThemeDataByIndex(int index) => themeDatas[index]; 
    private ColorThemeData GetThemeDataByTheme(ColorTheme theme) => themeDatas.FirstOrDefault(themeData => themeData.Theme == theme); 
    public void SetTheme(ColorTheme theme)
    {
        CurrentColorTheme = theme;
        PlayerPrefs.SetInt(COLOR_THEME_ID, GetIndexByTheme(theme));

        if(theme != ColorTheme.BLACK_AND_WHITE)
            ApplyColorThemeWithReplacement(theme);
        else
        {
            ColorThemeData blackThemeData = GetThemeDataByTheme(theme);
            colorAdjustments.active = false;
            colorReplacementFeature.SetActive(false);
        }
    }

    private void ApplyColorThemeWithReplacement(ColorTheme theme)
    {
        ColorThemeData themeData = GetThemeDataByTheme(theme);
        colorAdjustments.active = true;
        colorAdjustments.colorFilter.value = themeData.BackgroundColor;
        colorReplacementFeature.SetActive(true);
        colorReplacementFeature.settings.replacementColor = themeData.OutlineColor;
    }

    public void SetTheme(int index)
    {
        var themeData = GetThemeDataByIndex(index);
        SetTheme(themeData.Theme);
    }

}
