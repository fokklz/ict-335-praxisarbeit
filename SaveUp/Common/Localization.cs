using PropertyChanged;
using SaveUp.Models;
using SaveUp.Common.Events;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Resources;

namespace SaveUp.Common
{
    public class Localization : INotifyPropertyChanged
    {
        private static ResourceManager _resourceManager;

        /// <summary>
        /// Accessor for the current culture of the app
        /// Also updates the flyout item titles when the culture is changed since they don't update automatically
        /// </summary>
        public CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// All supported languages with their corresponding culture code
        /// </summary>
        public static Dictionary<string, string> LanguageMap = new Dictionary<string, string>
        {
            {"Deutsch", "de"},
            {"English", "en"},
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
            _resourceManager = Resources.Strings.Strings.ResourceManager;
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
            if (language == null || !LanguageMap.TryGetValue(language, out code))
            {
                code = "de";
            }
            var culture = new CultureInfo(code);
            Debug.WriteLine($"Localization: Setting language to {language} ({code})");
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
            // not sure if tabs will update, flyout did not
            /*if (Shell.Current?.Items is null)
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
            }*/
        }

        /// <summary>
        /// Get the resource from the resource manager with the given key
        /// Will return the key if the resource is not found or something goes wrong
        /// </summary>
        /// <param name="key">The key of the value to resolve</param>
        /// <returns>The resolved value as string</returns>
        public string GetResource(string key)
        {
            try { 
                // Try to get the string for the current culture
                string result = _resourceManager.GetString(key, CurrentCulture);

                // If not found, fallback to the default resource file
                if (string.IsNullOrEmpty(result))
                {
                    result = _resourceManager.GetString(key, CultureInfo.InvariantCulture);
                }

                if (string.IsNullOrEmpty(result))
                {
                    // If still not found, return the key
                    var splitted = key.Split('.');
                    return splitted.Length >= 2 ? splitted[splitted.Length - 2] : splitted[splitted.Length - 1];
                }

                return result;

            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }

            return key;
        }

        public ObservableCollection<PickerItem<string>> GetThemeDropdown()
        {
            return new ObservableCollection<PickerItem<string>> {
            new PickerItem<string> { DisplayText = Instance.Settings_Theme_System, BackgroundValue = "System" },
            new PickerItem<string> { DisplayText = Instance.Settings_Theme_Dark, BackgroundValue = "Dark" },
            new PickerItem<string> { DisplayText = Instance.Settings_Theme_Light, BackgroundValue = "Light" }
        };
        } 

        #region Dialogs
        [DependsOn(nameof(CurrentCulture))]
        public string Dialog_Ok => GetResource("Dialog.Ok");

        [DependsOn(nameof(CurrentCulture))]
        public string Dialog_No => GetResource("Dialog.No");

        [DependsOn(nameof(CurrentCulture))]
        public string Dialog_Yes => GetResource("Dialog.Yes");

        [DependsOn(nameof(CurrentCulture))]
        public string Dialog_CreatedItem_Title => GetResource("Dialog.CreatedItem.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string Dialog_CreatedItem_Message => GetResource("Dialog.CreatedItem.Message");
        #endregion

        #region App
        [DependsOn(nameof(CurrentCulture))]
        public string App_Title => GetResource("App.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time => GetResource("App.Time");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Day => GetResource("App.Time.Day");
        
        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Days => GetResource("App.Time.Days");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Hour => GetResource("App.Time.Hour");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Hours => GetResource("App.Time.Hours");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Minute => GetResource("App.Time.Minute");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Minutes => GetResource("App.Time.Minutes");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Second => GetResource("App.Time.Second");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Time_Seconds => GetResource("App.Time.Seconds");

        [DependsOn(nameof(CurrentCulture))]
        public string App_Imprint => GetResource("App.Imprint");

        [DependsOn(nameof(CurrentCulture))]
        public string App_PrivacyPolicy => GetResource("App.PrivacyPolicy"); 
        #endregion

        #region LoginPage
        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Login_Title => GetResource("AppLogin.Login.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Login_ForgotPassword => GetResource("AppLogin.Login.ForgotPassword");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Login_Submit => GetResource("AppLogin.Login.Submit");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Login_Register => GetResource("AppLogin.Login.Register");
        #endregion

        #region RegisterPage
        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Register_Title => GetResource("AppLogin.Register.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Register_Submit => GetResource("AppLogin.Register.Submit");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Register_Disclaimer => GetResource("AppLogin.Register.Disclaimer");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_Register_Login => GetResource("AppLogin.Register.Login");
        #endregion

        #region PasswordResetPage
        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_PasswordReset_Title => GetResource("AppLogin.PasswordReset.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_PasswordReset_Info => GetResource("AppLogin.PasswordReset.Info");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_PasswordReset_CodeButton => GetResource("AppLogin.PasswordReset.CodeButton");

        [DependsOn(nameof(CurrentCulture))]
        public string AppLogin_PasswordReset_Login => GetResource("AppLogin.PasswordReset.Login");
        #endregion

        #region Forms
        [DependsOn(nameof(CurrentCulture))]
        public string Form_Username => GetResource("Form.Username");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_Email => GetResource("Form.Email");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_Password => GetResource("Form.Password");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_ConfirmPassword => GetResource("Form.ConfirmPassword");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_Product => GetResource("Form.Product");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_Price => GetResource("Form.Price");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_Description => GetResource("Form.Description");

        [DependsOn(nameof(CurrentCulture))]
        public string Form_Clear => GetResource("Form.Clear");
        #endregion

        #region HomePage
        [DependsOn(nameof(CurrentCulture))]
        public string Home_Details_Remove => GetResource("Home.Details.Remove");
        #endregion

        #region AddPage
        [DependsOn(nameof(CurrentCulture))]
        public string Add_Title => GetResource("Add.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string Add_Submit => GetResource("Add.Submit");
        #endregion

        #region AboutPage
        [DependsOn(nameof(CurrentCulture))]
        public string About_Title => GetResource("About.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string About_Devs_Title => GetResource("About.Devs.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string About_Button_Settings => GetResource("About.Button.Settings");

        [DependsOn(nameof(CurrentCulture))]
        public string About_Button_Donate => GetResource("About.Button.Donate");
        #endregion

        #region SettingsPage
        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Title => GetResource("Settings.Title");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Currency_Label => GetResource("Settings.Currency.Label");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Language_Label => GetResource("Settings.Language.Label");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Theme_Label => GetResource("Settings.Theme.Label");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Theme_System => GetResource("Settings.Theme.System");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Theme_Dark => GetResource("Settings.Theme.Dark");
        
        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Theme_Light => GetResource("Settings.Theme.Light");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Button_Timespan => GetResource("Settings.Button.Timespan");

        [DependsOn(nameof(CurrentCulture))]
        public string Settings_Button_Logout => GetResource("Settings.Button.Logout");
        #endregion


    }
}
