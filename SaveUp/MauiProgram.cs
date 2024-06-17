using Microsoft.Extensions.Logging;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUp.LoginApp.ViewModels;
using SaveUp.LoginApp.Views;
using SaveUp.Services;
using SaveUp.Services.API;

namespace SaveUp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });


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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
