using SaveUp.Common;
using SaveUp.Views;
namespace SaveUp.ViewModels
{
    public class AboutViewModel : BaseNotifyHandler
    {
        /// <summary>
        /// Command to open a URL in the default browser.
        /// </summary>
        public Command OpenUrlCommand { get; }

        /// <summary>
        /// Command to open the settings page.
        /// </summary>
        public Command OpenSettingsCommand { get; }

        /// <summary>
        /// The version of the app.
        /// </summary>
        public string Version { get; set; } = App.VERSION;

        public AboutViewModel()
        {
            OpenUrlCommand = new Command<string>(async (url) => await OpenUrl(url));
            OpenSettingsCommand = new Command(async () =>
            {
                var viewModel = App.ServiceProvider.GetService<SettingsViewModel>()!;
                var settings = new SettingsPage(viewModel);
                var mainPage = App.Current?.MainPage;
                if (mainPage != null)
                {
                    await mainPage.Navigation.PushAsync(settings);
                }
            });
        }

        /// <summary>
        /// Opens a URL in the default browser.
        /// </summary>
        /// <param name="url">The URL to open.</param>
        /// <returns></returns>
        public async Task OpenUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return;
            }

            await Launcher.OpenAsync(uri);
        }
    }
}
