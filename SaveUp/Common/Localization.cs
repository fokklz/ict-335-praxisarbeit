using PropertyChanged;
using SaveUp.Common.Events;
using SaveUp.Common.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace SaveUp.Common
{
    public class Localization
    {
        /*/// <summary>
        /// All supported languages with their corresponding culture code
        /// </summary>
        public static Dictionary<string, string> LanguageMap = new Dictionary<string, string>
        {
            {"العربية", "ar"},
            {"Deutsch", "de"},
            {"English", "en"},
            {"Español", "es"},
            {"Français", "fr"},
            {"Italiano", "it"},
            {"Nederlands", "nl"},
            {"Polski", "pl"},
            {"Português", "pt"},
            {"Русский", "ru"},
            {"Türkçe", "tr"},
            {"한국어", "ko"},
            {"日本語", "ja"},
            {"中文(简体)", "zh-Hans"},
            {"中文(繁體)", "zh-Hant"},
            {"Svenska", "sv"},
            {"Dansk", "da"},
            {"Suomi", "fi"},
            {"Norsk", "no"},
            {"Čeština", "cs"},
            {"Magyar", "hu"},
            {"Ελληνικά", "el"},
            {"עברית", "he"},
            {"ไทย", "th"},
            {"हिंदी", "hi"},
            {"Български", "bg"},
            {"Română", "ro"},
            {"Українська", "uk"},
            {"Hrvatski", "hr"},
            {"Slovenský", "sk"},
            {"Lietuvių", "lt"},
            {"Slovenščina", "sl"},
            {"Eesti", "et"},
            {"Latviešu", "lv"},
            {"Tiếng Việt", "vi"},
            {"Bahasa Indonesia", "id"},
            {"Filipino", "fil"},
            {"Bahasa Melayu", "ms"}
        };

        /// <summary>
        /// Event that is invoked when the language is changed
        /// </summary>
        public static EventHandler<LanguageChangedEventArgs> LanguageChanged;

        /// <summary>
        /// Singleton instance of the Localization class
        /// </summary>
        public static Localization Instance { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Localization()
        {
            Instance = this;
        }

        protected virtual void OnLanguageChanged(string newLanguage, string code)
        {
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(newLanguage, code));
        }

        /// <summary>
        /// Allows to set the language of the app
        /// </summary>
        /// <param name="language">The language to use</param>
        public static void SetLanguage(string language)
        {
            string code;
            if (!LanguageMap.TryGetValue(language, out code))
            {
                code = "de";
            }
            var culture = new CultureInfo(code);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Instance.CurrentCulture = culture;
            UpdateFlyoutItemTitles();
            LanguageChanged?.Invoke(Instance, new LanguageChangedEventArgs(language, code));
        }

        /// <summary>
        /// Small helper method to update the titles of the flyout items
        /// since they for some reason don't update automatically with INotifyPropertyChanged
        /// we utilize the automation id to get the correct resource key
        /// </summary>
        private static void UpdateFlyoutItemTitles()
        {
            if (Shell.Current?.Items is null)
                return;

            foreach (var item in Shell.Current.Items)
            {
                if (item is FlyoutItem flyoutItem)
                {
                    var id = flyoutItem.AutomationId;
                    if (id is null)
                        continue;
                    flyoutItem.Title = Instance.GetResource(id.Replace('_', '.'));
                }
            }
        }

        /// <summary>
        /// Get the resource from the resource manager with the given key
        /// Will return the key if the resource is not found or something goes wrong
        /// </summary>
        /// <param name="key">The key of the value to resolve</param>
        /// <returns>The resolved value as string</returns>
        public string GetResource(string key)
        {
            return ResourceManager.GetLanguageResource(key, CurrentCulture);
        }

        /// <summary>
        /// Accessor for the current culture of the app
        /// Also updates the flyout item titles when the culture is changed since they don't update automatically
        /// </summary>
        public CultureInfo CurrentCulture { get; set; }

        [DependsOn(nameof(CurrentCulture))]
        public ObservableCollection<PickerIte<string>> ThemeDropdown => new ObservableCollection<PickerItem<string>> {
            new PickerItem<string> { DisplayText = Instance.SettingsPage_Theme_System, BackgroundValue = "System" },
            new PickerItem<string> { DisplayText = Instance.SettingsPage_Theme_Dark, BackgroundValue = "Dark" },
            new PickerItem<string> { DisplayText = Instance.SettingsPage_Theme_Light, BackgroundValue = "Light" }
        };

        [DependsOn(nameof(CurrentCulture))]
        public string OrderList_Search => GetResource("OrderList.Search");
        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_LastLoginsLabel => GetResource("AppLogin.LastLoginsLabel");
        [DependsOn(nameof(CurrentCulture))]*/

    }
}
