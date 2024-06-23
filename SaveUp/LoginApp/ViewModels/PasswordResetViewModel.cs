using SaveUp.Common;
using SaveUp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaveUp.LoginApp.ViewModels
{
    public class PasswordResetViewModel : BaseNotifyHandler
    {

        private readonly IAlertService _alertService;

        public bool ShowEmailEntry { get; set; } = false;

        public bool ShowCodeEntry { get; set; } = false;
    
        public bool IsEmailStepVisible { get; set; } = true;

        public bool CodeValidated { get; set; } = false;

        public string Email { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public Command SubmitEmail { get; } 

        public Command SubmitCode { get; }

        public Command SubmitPassword { get; }

        public Command GotoLogin { get; } = new Command(() => {
            Shell.Current.GoToAsync("//Login");
        });

        public PasswordResetViewModel(IAlertService alertService) { 
            _alertService = alertService;
            SubmitEmail = new Command(OnEmailSubmit);
            SubmitCode = new Command(OnCodeSubmit);
            SubmitPassword = new Command(OnPasswordSubmit);
        }

        private void OnEmailSubmit()
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            bool isEmailValid = Regex.IsMatch(Email, emailPattern);

            if (isEmailValid)
            {
                // Email is valid, proceed with sending the email
                // TODO: Send email with code
                IsEmailStepVisible = false;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await _alertService.ShowAsync("Invalid Email", "Please enter a valid email address");
                });
            }
        }

        private void OnCodeSubmit()
        {
            //CodeValidated = true;
        }

        private void OnPasswordSubmit()
        {

        }
    
    }
}
