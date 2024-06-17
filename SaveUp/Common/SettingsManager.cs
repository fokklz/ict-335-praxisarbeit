using SaveUp.Common.Helpers;
using System.Diagnostics;

namespace SaveUp.Common
{
    public static class SettingsManager
    {
        /// <summary>
        /// Current language setting
        /// </summary>
        public static string Language { get; private set; }

        /// <summary>
        /// Current theme setting
        /// </summary>
        public static string Theme { get; private set; }

        /// <summary>
        /// Current cancel in list view setting
        /// </summary>
        public static bool CancelInListView { get; private set; }

        /// <summary>
        /// Current always save login setting
        /// </summary>
        public static bool AlwaysSaveLogin { get; private set; }

        /// <summary>
        /// The preferences API to use for storing settings
        /// - needed for unit testing
        /// </summary>
        public static IPreferences PreferencesAPI { get; set; } = Preferences.Default;

        /// <summary>
        /// Load settings from local storage (user based)
        /// </summary>
        public static void LoadSettings(string? userId = null)
        {
            Language = PreferencesAPI.Get($"{SettingsKey.Language}.{userId ?? AuthManager.UserId}", "Deutsch");
            Debug.WriteLine($"Loaded language: {Language} ---------------------------------------------------------------");
            Theme = PreferencesAPI.Get($"{SettingsKey.Theme}.{userId ?? AuthManager.UserId}", "System");
            CancelInListView = PreferencesAPI.Get($"{SettingsKey.CancelInListView}.{userId ?? AuthManager.UserId}", false);
            AlwaysSaveLogin = PreferencesAPI.Get($"{SettingsKey.AlwaysSaveLogin}.{userId ?? AuthManager.UserId}", false);
        }

        /// <summary>
        /// Set the language setting for the current logged in user
        /// </summary>
        /// <param name="language">The language to set, should be contained in the languageMap in Localization</param>
        public static void SetLanguage(string language)
        {
            Language = language;
            PreferencesAPI.Set($"{SettingsKey.Language}.{AuthManager.UserId}", language);
            ApplyLanguage();
        }

        /// <summary>
        /// Set the theme setting for the current logged in user
        /// </summary>
        /// <param name="theme">The theme to set, should match the common english Names (Dark, Light, System)</param>
        public static void SetTheme(string theme)
        {
            Theme = theme;
            PreferencesAPI.Set($"{SettingsKey.Theme}.{AuthManager.UserId}", theme);
            ApplyTheme();
        }

        /// <summary>
        /// Set the cancel in list view setting for the current logged in user
        /// </summary>
        /// <param name="cancelInListView">The value to set, should be true when cancel should be shown in the list view</param>
        public static void SetCancelInListView(bool cancelInListView)
        {
            CancelInListView = cancelInListView;
            PreferencesAPI.Set($"{SettingsKey.CancelInListView}.{AuthManager.UserId}", cancelInListView);
        }

        /// <summary>
        /// Set the always save login setting for the current logged in user
        /// </summary>
        /// <param name="alwaysSaveLogin">The value to set, should be true to skip the logout dialog</param>
        public static void SetAlwaysSaveLogin(bool alwaysSaveLogin)
        {
            AlwaysSaveLogin = alwaysSaveLogin;
            PreferencesAPI.Set($"{SettingsKey.AlwaysSaveLogin}.{AuthManager.UserId}", alwaysSaveLogin);
        }

        public static void ApplyTheme()
        {
            var app = Application.Current;
            if (app != null)
            {
                app.UserAppTheme = Theme switch
                {
                    "Dark" => AppTheme.Dark,
                    "Light" => AppTheme.Light,
                    _ => AppTheme.Unspecified
                };
            }
        }

        public static void ApplyLanguage()
        {
            //Localization.SetLanguage(Language);
        }

        public static void ApplySettings()
        {
            ApplyTheme();
            ApplyLanguage();
        }
    }
}
