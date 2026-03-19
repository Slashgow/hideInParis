using inkolorgames;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class UILanguage : MonoSingleton<UILanguage>
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private void Start()
    {
        PopulateDropdown();
        LoadSettings();
    }

    private void PopulateDropdown()
    {
        languageDropdown.ClearOptions();

#if UNITY_WEBGL
        LocalizationSettings.InitializationOperation.Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                int localeCount = LocalizationSettings.AvailableLocales.Locales.Count;
                var locales = LocalizationSettings.AvailableLocales.Locales;

                var localeOptions = new System.Collections.Generic.List<string>();
                for (int i = 0; i < localeCount; i++)
                {
                    localeOptions.Add(locales[i].LocaleName);
                }
                languageDropdown.AddOptions(localeOptions);
            }
        };
#endif

#if !UNITY_WEBGL
        int localeCount = LocalizationSettings.AvailableLocales.Locales.Count;
        var locales = LocalizationSettings.AvailableLocales.Locales;

        var localeOptions = new System.Collections.Generic.List<string>();
        for (int i = 0; i < localeCount; i++)
        {
            localeOptions.Add(locales[i].LocaleName);
        }
        languageDropdown.AddOptions(localeOptions);
#endif

    }

    private void LoadSettings()
    {
#if UNITY_WEBGL
        LocalizationSettings.InitializationOperation.Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                int currentLocaleIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
                languageDropdown.value = currentLocaleIndex >= 0 ? currentLocaleIndex : 0; // Default to first locale if not found
            }
        };
#endif

#if !UNITY_WEBGL
        int currentLocaleIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        languageDropdown.value = currentLocaleIndex >= 0 ? currentLocaleIndex : 0; // Default to first locale if not found
#endif

    }

    public void SetLanguage(int index)
    {
        LanguageManager.Instance.SetLanguage(index);
    }

}
