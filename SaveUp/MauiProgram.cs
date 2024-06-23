using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUp.LoginApp.ViewModels;
using SaveUp.LoginApp.Views;
using SaveUp.Services;
using SaveUp.Services.API;
using SaveUp.ViewModels;
using SaveUp.Views;
using System.Diagnostics;

#if ANDROID
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
#endif

namespace SaveUp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


            builder.Services.AddSingleton<IStorageService, StorageService>();
            // all api services need the auth service
            builder.Services.AddSingleton<IAuthService, AuthService>();

            builder.Services.AddSingleton<IAlertService, AlertService>();

            builder.Services.AddSingleton<IItemAPIService, ItemAPIService>();
            builder.Services.AddSingleton<IUserAPIService, UserAPIService>();

            // Login App
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<LoginPage>();

            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<RegisterPage>();

            builder.Services.AddSingleton<PasswordResetViewModel>();
            builder.Services.AddSingleton<PasswordResetPage>();

            //Main Application
            builder.Services.AddSingleton<AboutViewModel>();
            builder.Services.AddSingleton<AboutPage>();

            builder.Services.AddSingleton<AddViewModel>();
            builder.Services.AddSingleton<AddPage>();

            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<HomePage>();

            builder.Services.AddSingleton<HomeDetailsViewModel>();
            builder.Services.AddSingleton<HomeDetailsPage>();

            builder.Services.AddTransient<SettingsViewModel>();

            // Core App Shell
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.ConfigureLifecycleEvents(lifecycle =>
            {

#if ANDROID
                lifecycle.AddAndroid(android =>
                {
                    android.OnCreate(async (activity, bundle) => {
                        Debug.WriteLine("Lifecycle: OnCreate ------");

                        var storageService = App.ServiceProvider.GetService<IStorageService>();
                        var authService = App.ServiceProvider.GetService<IAuthService>();

                        await storageService?.InitializeAsync();
                        await authService?.TryLoginFromStorage();
                    });

                    android.OnResume(activity => {
                        Debug.WriteLine("Lifecycle: OnResume ------");
                    });

                    android.OnStop(activity => {
                        Debug.WriteLine("Lifecycle: OnStop ------");
                    });
                });

#endif

#if IOS
                lifecycle.AddiOS(ios =>
                {
                    ios.FinishedLaunching((app, options) => {
                        Debug.WriteLine("Lifecycle: FinishedLaunching ------");

                        var storageService = App.ServiceProvider.GetService<IStorageService>();
                        var authService = App.ServiceProvider.GetService<IAuthService>();

                        storageService.InitializeAsync();
                        authService.TryLoginFromStorage();

                        return true;

                    });

                    ios.WillEnterForeground(app => {
                        Debug.WriteLine("Lifecycle: WillEnterForeground ------");
                    });

                    ios.DidEnterBackground(app => {
                        Debug.WriteLine("Lifecycle: DidEnterBackground ------");
                    });
                });
#endif
            });

            // to remove the switch labels on win ui since they don't support multi language -> fk microsoft
#if WINDOWS
            Microsoft.Maui.Handlers.SwitchHandler.Mapper.AppendToMapping("NoLabel", (handler, view) =>
            {
                handler.PlatformView.OnContent = null;
                handler.PlatformView.OffContent = null;
                handler.PlatformView.MinWidth = 0;
            });

#endif

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextViewBorderStyle.None;
#endif
            });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            });


            return builder.Build();
        }
    }
}
