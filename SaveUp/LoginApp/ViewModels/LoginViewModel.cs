using SaveUp.Common;

namespace SaveUp.LoginApp.ViewModels
{
    public class LoginViewModel : BaseNotifyHandler
    {

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;

        public bool IsSuccess { get; set; } = false;

        public string Message { get; set; } = string.Empty;

        public Command GotoRegister { get; set; } = new Command(() => {
            Shell.Current.GoToAsync("//Register");
        });

        public Command GotoPasswordReset { get; set; } = new Command(() => {
            Shell.Current.GoToAsync("//PasswordReset");
        });

        public LoginViewModel()
        {
        }

        public void ResetLoginState()
        {
            IsSuccess = false;
            Message = string.Empty;
            Password = string.Empty;
        }



    }
}
