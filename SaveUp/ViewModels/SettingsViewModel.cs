using PropertyChanged;
using SaveUp.Common;
using SaveUp.Common.Events;
using SaveUp.Interfaces;
using SaveUp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SaveUp.ViewModels
{
    public class SettingsViewModel : BaseNotifyHandler, IDisposable
    {
        private readonly IAlertService _alertService;
        private readonly IAuthService _authService;

        /// <summary>
        /// The command to reset the time span
        /// </summary>
        public Command ResetTimeSpanCommand { get; }

        /// <summary>
        /// The command to logout
        /// </summary>
        public Command LogoutCommand { get; }

        /// <summary>
        /// The dropdown for the themes
        /// </summary>
        public ObservableCollection<PickerItem<string>> Themes { get; set; } = new ObservableCollection<PickerItem<string>>();

        /// <summary>
        /// The dropdown for the languages
        /// </summary>
        public ObservableCollection<string> Languages { get; set; } = new ObservableCollection<string>(Localization.LanguageMap.Keys);

        /// <summary>
        /// The currency to use
        /// </summary>
        [OnChangedMethod(nameof(ChangeCurrency))]
        public string Currency { get; set; } = SettingsManager.Currency;

        /// <summary>
        /// The selected theme
        /// </summary>
        [OnChangedMethod(nameof(ChangeTheme))]
        public PickerItem<string> SelectedTheme { get; set; }

        /// <summary>
        /// The selected language
        /// </summary>
        [OnChangedMethod(nameof(ChangeLanguage))]
        public string SelectedLanguage { get; set; } = SettingsManager.Language;


        public SettingsViewModel(IAlertService alertService, IAuthService authService)
        {
            _alertService = alertService;
            _authService = authService;

            ResetTimeSpanCommand = new Command(async () => await ResetTimeSpan());
            LogoutCommand = new Command(async () => await Logout());


            LanguageChanged(null, null);
            Localization.LanguageChanged += LanguageChanged;

            Debug.WriteLine("SettingsViewModel created");
        }

        /// <summary>
        /// Dispose the view model
        /// </summary>
        public void Dispose()
        {
            Localization.LanguageChanged -= LanguageChanged;
            Debug.WriteLine("SettingsViewModel disposed");
        }

        /// <summary>
        /// Subscribe to the language changed event to reflect changes in the dropdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void LanguageChanged(object? sender, LanguageChangedEventArgs? args)
        {
            var dropdown = Localization.Instance.GetThemeDropdown();
            Themes.Clear();
            foreach (var item in dropdown)
            {
                Themes.Add(item);
            }
            SelectedTheme = Themes.FirstOrDefault(x => x.BackgroundValue == SettingsManager.Theme)
                            ?? dropdown[0];
        }

        /// <summary>
        /// Reset the time span
        /// </summary>
        /// <returns></returns>
        private async Task ResetTimeSpan()
        {
            var result = await _alertService.ConfirmAsync("ResetTimeSpanTitle", "ResetTimeSpanMessage");
            if (result)
            {
                SettingsManager.SetTimeSpan();
            }
        }

        /// <summary>
        /// Logout the user
        /// </summary>
        /// <returns></returns>
        private async Task Logout()
        {
            await _authService.LogoutAsync();
        }

        /// <summary>
        /// Change handler for the theme
        /// </summary>
        private void ChangeTheme()
        {
            if (SelectedTheme == null) return;
            SettingsManager.SetTheme(SelectedTheme.BackgroundValue);
        }
        
        /// <summary>
        /// Change handler for the language
        /// </summary>
        private void ChangeLanguage()
        {
            SettingsManager.SetLanguage(SelectedLanguage);
        }

        /// <summary>
        /// Change handler for the currency
        /// </summary>
        private void ChangeCurrency()
        {
            SettingsManager.SetCurrency(Currency);
        }

    }
}
