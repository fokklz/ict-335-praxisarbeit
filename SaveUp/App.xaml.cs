using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.LoginApp;
using System.Diagnostics;

namespace SaveUp
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;

        public static String VERSION = "1.0.0";

        public static IServiceProvider ServiceProvider { get; set; }

        public AppShell MainAppShell { get; private set; }

        public LoginAppShell MainAppLogin { get; private set; }

        public App(IServiceProvider serviceProvider, IAuthService authService, IStorageService storageService)
          {
            ServiceProvider = serviceProvider;
            _authService = authService;

            InitializeComponent();

            MainAppShell = serviceProvider.GetService<AppShell>();
            MainAppLogin = new LoginAppShell();
#if ANDROID || IOS
            MainAppLogin.Opacity = 0;
#endif
            // ensure the login page will only turn visible with the animation
            // setting opacity to 0 will hide it until the animation is called
            MainPage = MainAppLogin;

            AuthManager.LoginChanged += async (s, e) =>
            {
                SettingsManager.LoadSettings(e.UserId);

                if (e.IsLoggedIn)
                {
                    await SwitchToMainApp();
                }
                else
                {
                    await SwitchToLogin();
                }

                SettingsManager.ApplySettings();
            };

            if (AuthManager.IsLoggedIn)
            {
                SwitchToMainApp();
            }
            else
            {
                SwitchToLogin();
            }
        
        }

        /// <summary>
        /// Switch to the main app
        /// </summary>
        /// <returns>Nothing</returns>
        public async Task SwitchToMainApp()
        {
            await _animatePageTransition(MainAppShell, isAppearing: true);
            MainPage = MainAppShell;
            await _animatePageTransition(MainAppShell, isAppearing: false);
        }

        /// <summary>
        /// Switch to the login page
        /// </summary>
        /// <returns>Nothing</returns>
        public async Task SwitchToLogin()
        {
            // If the user is already logged in, there is no need to show the login page
            if (AuthManager.IsLoggedIn)
            {
                return;
            }

            MainAppLogin = new LoginAppShell();
#if ANDROID || IOS
            MainAppLogin.Opacity = 0;
#endif
            await _animatePageTransition(MainAppLogin, isAppearing: true);
            MainPage = MainAppLogin;
            await _animatePageTransition(MainAppLogin, isAppearing: false);
        }

        /// <summary>
        /// Helper to animate the page transition
        /// </summary>
        /// <param name="newPage">The page that should appear next</param>
        /// <param name="isAppearing">If it already has been changed (MainPage)</param>
        /// <returns>Nothing</returns>
        private async Task _animatePageTransition(Page newPage, bool isAppearing)
        {

#if ANDROID || IOS
            if (isAppearing)
            {
                // Fade out and scale down the current page
                if (MainPage != null)
                {
                    await Task.WhenAll(
                        MainPage.FadeTo(0, 250),
                        MainPage.ScaleTo(0.9, 250)
                    );
                }
            }
            else
            {
                newPage.AnchorX = 0.52;
                newPage.AnchorY = 0.52;

                // Reset scale and fade in the new page
                newPage.Opacity = 0;
                newPage.Scale = 0.9;

                await Task.WhenAll(
                    newPage.FadeTo(1, 250),
                    newPage.ScaleTo(1, 250)
                );
            }
#endif
        }
    }
}
