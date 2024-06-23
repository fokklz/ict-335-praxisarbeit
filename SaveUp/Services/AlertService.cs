using SaveUp.Common;
using SaveUp.Interfaces;

namespace SaveUp.Services
{
    /// <summary>
    /// Service to show alerts.
    /// </summary>
    public class AlertService : IAlertService
    {

        /// <summary>
        /// Display an alert on the MainPage of the application.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message of the alert.</param>
        /// <param name="cancel">The cancel button text of the alert.</param>
        /// <returns></returns>
        public async Task ShowAsync(string title, string message)
        {
            var mainPage = Application.Current?.MainPage;
            if (mainPage == null)
            {
                return;
            }

            await mainPage.DisplayAlert(title, message, Localization.Instance.Dialog_Ok)!;
        }

        /// <summary>
        /// Display a confirmation alert on the MainPage of the application.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message of the alert.</param>
        /// <returns>True if the user confirmed the alert, false otherwise.</returns>
        public async Task<bool> ConfirmAsync(string title, string message)
        {
            var mainPage = Application.Current?.MainPage;
            if (mainPage == null)
            {
                return false;
            }

            return await mainPage.DisplayAlert(title, message, Localization.Instance.Dialog_Yes, Localization.Instance.Dialog_No);
        }
    }
}
