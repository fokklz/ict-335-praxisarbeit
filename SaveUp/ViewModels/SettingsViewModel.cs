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

        public Command ResetTimeSpanCommand { get; }

        public Command LogoutCommand { get; }

        public ObservableCollection<PickerItem<string>> Themes { get; set; } = new ObservableCollection<PickerItem<string>>();
        public ObservableCollection<string> Languages { get; set; } = new ObservableCollection<string>(Localization.LanguageMap.Keys);

        [OnChangedMethod(nameof(ChangeCurrency))]
        public string Currency { get; set; } = SettingsManager.Currency;

        [OnChangedMethod(nameof(ChangeTheme))]
        public PickerItem<string> SelectedTheme { get; set; }

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

        public void Dispose()
        {
            Localization.LanguageChanged -= LanguageChanged;
            Debug.WriteLine("SettingsViewModel disposed");
        }

        private void LanguageChanged(object? sender, LanguageChangedEventArgs? args)
        {
            var dropdown = Localization.Instance.GetThemeDropdown();
            foreach (var item in dropdown)
            {
                var existingItem = Themes.FirstOrDefault(x => x.BackgroundValue == item.BackgroundValue);
                if (existingItem == null)
                {
                    Themes.Add(item);
                }
                else
                {
                    existingItem.DisplayText = item.DisplayText;
                }
            }
            SelectedTheme = Themes.FirstOrDefault(x => x.BackgroundValue == SettingsManager.Theme)
                            ?? dropdown[0];
        }

        private async Task ResetTimeSpan()
        {
            var result = await _alertService.ConfirmAsync("ResetTimeSpanTitle", "ResetTimeSpanMessage");
            if (result)
            {
                SettingsManager.SetTimeSpan();
            }
        }

        private async Task Logout()
        {
            await _authService.LogoutAsync();
        }

        private void ChangeTheme()
        {
            SettingsManager.SetTheme(SelectedTheme.BackgroundValue);
        }

        private void ChangeLanguage()
        {
            SettingsManager.SetLanguage(SelectedLanguage);
        }

        private void ChangeCurrency()
        {
            SettingsManager.SetCurrency(Currency);
        }

    }
}
