using PropertyChanged;
using SaveUp.Common;

namespace SaveUp.LoginApp.ViewModels
{
    public class RegisterViewModel : BaseNotifyHandler
    {

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [OnChangedMethod(nameof(OnConfirmPasswordChange))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsSuccess { get; set; } = false;

        public string Message { get; set; } = string.Empty;

        public Command GotoLogin { get; set; } = new Command(() => {
            Shell.Current.GoToAsync("//Login");
        });

        public RegisterViewModel()
        {
        }

        public void ResetRegisterState()
        {
            IsSuccess = false;
            Message = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        public void OnConfirmPasswordChange()
        {
            if (Password != ConfirmPassword)
            {
                Message = "Password and Confirm Password do not match";
            }
            else
            {
                Message = string.Empty;
            }
        }


    }
}
