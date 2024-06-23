using PropertyChanged;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using System.Diagnostics;

namespace SaveUp.LoginApp.ViewModels
{
    public class RegisterViewModel : BaseNotifyHandler
    {
        private readonly IAuthService _authService;
        private readonly IAlertService _alertService;
        private readonly IUserAPIService _userAPIService;
        private readonly IStorageService _storageService;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [OnChangedMethod(nameof(OnConfirmPasswordChange))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool ValidConfirmPassword { get; set; } = false;

        public bool IsSuccess { get; set; } = false;

        public Command GotoLoginCommand { get; }

        public Command RegisterCommand { get; }

        public RegisterViewModel(IAuthService authService, IAlertService alertService, IUserAPIService userAPIService, IStorageService storageService)
        {
            _authService = authService;
            _alertService = alertService;
            _userAPIService = userAPIService;
            _storageService = storageService;

            GotoLoginCommand = new Command(() => {
                Shell.Current.GoToAsync("//Login");
            });
            RegisterCommand = new Command(async () => await Register());

        }

        public void ResetRegisterState()
        {
            IsSuccess = false;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Username = string.Empty;
            Email = string.Empty;
        }

        public async void OnConfirmPasswordChange()
        {
            if (Password != ConfirmPassword && Password.Length <= ConfirmPassword.Length)
            {
                ValidConfirmPassword = false;
                var localKey = "Backend.PasswordMismatch";
                await _alertService.ShowAsync(Localization.Instance.GetResource($"{localKey}.Title"), Localization.Instance.GetResource($"{localKey}.Message"));
            }
            else
            {
                ValidConfirmPassword = Password == ConfirmPassword;
            }
        }

        public async Task Register()
        {
            Debug.WriteLine($"Registering user {Username} with email {Email} - {ValidConfirmPassword}");
            if (!ValidConfirmPassword)
            {
                OnConfirmPasswordChange();
                return;
            }

            var res = await _userAPIService.RegisterAsync(Username, Email, Password);
            if (res.IsSuccess)
            {
                // we directly process the token here, since we need to store it in the storage service
                var parsed = await res.ParseSuccess();
                if (parsed != null && parsed.Auth != null && parsed.Auth.Token != null)
                {
                    var refreshToken = parsed.Auth.RefreshToken;
                    if (refreshToken != null)
                    {
                        _storageService.StoreUser(parsed.Email, parsed.Auth.Token, refreshToken);
                        await _storageService.SaveChangesAsync();
                    }

                    AuthManager.Login(parsed.Auth.Token, refreshToken, parsed.Id);
                }
            }
            else
            {
                var error = await res.ParseError();
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
