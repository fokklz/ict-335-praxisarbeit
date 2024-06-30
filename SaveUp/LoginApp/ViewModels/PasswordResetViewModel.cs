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

        /// <summary>
        /// If the email entry should be shown
        /// </summary>
        public bool ShowEmailEntry { get; set; } = false;

        /// <summary>
        /// If the code entry should be shown
        /// </summary>
        public bool ShowCodeEntry { get; set; } = false;
    
        /// <summary>
        /// If current step is email
        /// </summary>
        public bool IsEmailStepVisible { get; set; } = true;

        /// <summary>
        /// If code was validated
        /// </summary>
        public bool CodeValidated { get; set; } = false;

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The code to reset the password
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The new password of the user
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// The confirmation password of the user
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Command to submit the email
        /// </summary>
        public Command SubmitEmailCommand { get; } 

        /// <summary>
        /// Command to submit the code
        /// </summary>
        public Command SubmitCodeCommand { get; }

        /// <summary>
        /// Command to submit the password
        /// </summary>
        public Command SubmitPasswordCommand { get; }

        public Command GotoLogin { get; } = new Command(() => {
            Shell.Current.GoToAsync("//Login");
        });

        public PasswordResetViewModel(IAlertService alertService) { 
            _alertService = alertService;
            SubmitEmailCommand = new Command(OnEmailSubmit);
            SubmitCodeCommand = new Command(OnCodeSubmit);
            SubmitPasswordCommand = new Command(OnPasswordSubmit);
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
