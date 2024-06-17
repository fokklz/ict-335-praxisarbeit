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
        public async Task ShowAlertAsync(string title, string message, string cancel)
        {
            await Application.Current?.MainPage?.DisplayAlert(title, message, cancel)!;
        }
    }
}
