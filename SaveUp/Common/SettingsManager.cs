using SaveUp.Common.Helpers;
using System.ComponentModel;
using System.Diagnostics;

namespace SaveUp.Common
{
    public static class SettingsManager 
    {

        public static event PropertyChangedEventHandler PropertyChanged;

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
        public static string Currency { get; private set; }

        /// <summary>
        /// Current always save login setting
        /// </summary>
        public static DateTime TimeSpan { get; private set; }

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
            Theme = PreferencesAPI.Get($"{SettingsKey.Theme}.{userId ?? AuthManager.UserId}", "System");
            Currency = PreferencesAPI.Get($"{SettingsKey.Currency}.{userId ?? AuthManager.UserId}", "CHF");
            TimeSpan = PreferencesAPI.Get($"{SettingsKey.TimeSpan}.{userId ?? AuthManager.UserId}", DateTime.UtcNow);
            Debug.WriteLine("SettingsManager: Settings loaded");
        }

        /// <summary>
        /// Set the language setting for the current logged in user
        /// </summary>
        /// <param name="language">The language to set, should be contained in the languageMap in Localization</param>
        public static void SetLanguage(string language)
        {
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                Language = language;
                PreferencesAPI.Set($"{SettingsKey.Language}.{AuthManager.UserId}", Language);
                ApplyLanguage();
                Debug.WriteLine($"SettingsManager: Language set to {Language}");
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Language)));
            });
        }

        /// <summary>
        /// Set the theme setting for the current logged in user
        /// </summary>
        /// <param name="theme">The theme to set, should match the common english Names (Dark, Light, System)</param>
        public static void SetTheme(string theme)
        {
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                Theme = theme;
                PreferencesAPI.Set($"{SettingsKey.Theme}.{AuthManager.UserId}", Theme);
                ApplyTheme();
                Debug.WriteLine($"SettingsManager: Theme set to {Theme}");
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Theme)));
            });
        }

        /// <summary>
        /// Set the currency setting for the current logged in user
        /// </summary>
        /// <param name="currency">The currency to set, should be a valid currency code</param>
        public static void SetCurrency(string currency)
        {
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                Currency = currency;
                PreferencesAPI.Set($"{SettingsKey.Currency}.{AuthManager.UserId}", Currency);
                Debug.WriteLine($"SettingsManager: Currency set to {Currency}");
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Currency)));
            });
        }

        /// <summary>
        /// Set the time span setting for the current logged in user
        /// </summary>
        public static void SetTimeSpan()
        {
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                TimeSpan = DateTime.Now;
                PreferencesAPI.Set($"{SettingsKey.TimeSpan}.{AuthManager.UserId}", TimeSpan);
                Debug.WriteLine("SettingsManager: TimeSpan set to now");
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(TimeSpan)));
            });
        }

        /// <summary>
        /// Evaluate if the current user has a active time span
        /// </summary>
        /// <returns>True if the user has a time span, false otherwise</returns>
        public static bool HasTimeSpan()
        {
            var eval = PreferencesAPI.ContainsKey($"{SettingsKey.TimeSpan}.{AuthManager.UserId}");
            Debug.WriteLine($"SettingsManager: HasTimeSpan {eval}");
            return eval;
        }

        /// <summary>
        /// Apply the current theme setting
        /// </summary>
        public static void ApplyTheme()
        {
            Application.Current?.Dispatcher.Dispatch(() =>
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
            });
        }

        /// <summary>
        /// Apply the current language setting
        /// </summary>
        public static void ApplyLanguage()
        {
            Localization.SetLanguage(Language);
        }


        /// <summary>
        /// Apply language and theme settings
        /// </summary>
        public static void ApplySettings()
        {
            ApplyTheme();
            ApplyLanguage();
        }
    }
}
