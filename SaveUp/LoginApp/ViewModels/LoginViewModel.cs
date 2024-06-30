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

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// If the user wants to be remembered (not changeable by now)
        /// </summary>
        public bool RememberMe { get; set; } = true;

        /// <summary>
        /// If the login was successful
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// Navigate to the register page
        /// </summary>
        public Command GotoRegisterCommand { get; set; }

        /// <summary>
        /// Navigate to the password reset page
        /// </summary>
        public Command GotoPasswordResetCommand { get; set; } 

        /// <summary>
        /// Command to login the user
        /// </summary>
        public Command LoginCommand { get; set; }

        /// <summary>
        /// Tracker for coming soon popup
        /// Always reset to 0 after 2, 2 = Showing alert
        /// </summary>
        public int Soon { get; set; } = 0;

        public LoginViewModel(IAuthService authService, IAlertService alertService)
        {
            _authService = authService;
            _alertService = alertService;

            GotoRegisterCommand = new Command(async () => {
                await Shell.Current.GoToAsync("//Register");
            });
            GotoPasswordResetCommand = new Command(async () => {
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

        /// <summary>
        /// Reset the login state
        /// </summary>
        public void ResetLoginState()
        {
            IsSuccess = false;
            Password = string.Empty;
            Email = string.Empty;
            Soon = 0;
        }

        /// <summary>
        /// Execute the login command with the current information
        /// </summary>
        /// <returns></returns>
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
