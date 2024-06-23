using SaveUp.Common;
using SaveUp.Common.Models;
using SaveUp.Interfaces;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.LoginApp.ViewModels
{
    public class LoginViewModel : BaseNotifyHandler
    {

        private readonly IAuthService _authService;
        private readonly IAlertService _alertService;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = true;

        public bool IsSuccess { get; set; } = false;

        public Command GotoRegister { get; set; }

        public Command GotoPasswordReset { get; set; } 

        public Command LoginCommand { get; set; }

        public int Soon { get; set; } = 0;

        public LoginViewModel(IAuthService authService, IAlertService alertService)
        {
            _authService = authService;
            _alertService = alertService;

            GotoRegister = new Command(async () => {
                await Shell.Current.GoToAsync("//Register");
            });
            GotoPasswordReset = new Command(async () => {
                Soon++;

                if (Soon == 2)
                {
                    await _alertService.ShowAsync("Coming Soon", "This feature is coming soon!");
                    Soon = 0;
                }
                //await Shell.Current.GoToAsync("//PasswordReset");
            });
            LoginCommand = new Command(async () => await Login());
        }

        public void ResetLoginState()
        {
            IsSuccess = false;
            Password = string.Empty;
            Email = string.Empty;
            Soon = 0;
        }

        public async Task Login()
        {
            var response = await _authService.LoginAsync(Email, Password, RememberMe);
            if (response.IsSuccess)
            {
                IsSuccess = true;
            }
            else
            {
                var error = await response.ParseError();
                if (error?.Errors != null)
                {
                    // We expect the backend to return a key for the error message
                    // This key should be in the format of "Backend.{key}.Title/Message"
                    // If the key is not found, we will show the last part of the key as message
                    var key = error.Errors.Keys.FirstOrDefault()!;
                    var localKey = $"Backend.{error.Errors[key][0]}";
                    await _alertService.ShowAsync(Localization.Instance.GetResource($"{localKey}.Title"), Localization.Instance.GetResource($"{localKey}.Message"));
                }
            }
        }


    }
}
