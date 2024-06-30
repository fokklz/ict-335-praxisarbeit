using MongoDB.Bson;
using PropertyChanged;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;

namespace SaveUp.ViewModels
{
    public class HomeDetailsViewModel : BaseNotifyHandler, IDisposable
    {
        private readonly IItemAPIService _itemAPIService;
        private readonly IAlertService _alertService;

        /// <summary>
        /// Currency to use
        /// </summary>
        public string Currency { get; set; } = SettingsManager.Currency;

        /// <summary>
        /// The name of the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the product
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The price of the product
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// The date the product was added
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The time span the product is part of
        /// </summary>
        public DateTime TimeSpan { get; set; }

        /// <summary>
        /// The id of the product
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The command to delete the item
        /// </summary>
        public Command DeleteCommand { get; }

        public HomeDetailsViewModel(IItemAPIService itemAPIService, IAlertService alertService)
        {
            _itemAPIService = itemAPIService;
            _alertService = alertService;

            DeleteCommand = new Command(async () => await Delete());

            SettingsManager.PropertyChanged += SettingsManager_PropertyChanged;
        }

        /// <summary>
        /// Dispose the view model
        /// </summary>
        public void Dispose()
        {
            SettingsManager.PropertyChanged -= SettingsManager_PropertyChanged;
        }

        /// <summary>
        /// Subscribe to the property changed event of the settings manager to reflect changes in the currency
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsManager.Currency))
            {
                Currency = SettingsManager.Currency;
            }
        }

        /// <summary>
        /// Delete the item
        /// </summary>
        /// <returns></returns>
        public async Task Delete()
        {
           var result = await _alertService.ConfirmAsync("Delete", "Are you sure you want to delete this item?");
            if (result)
            {
                await _itemAPIService.DeleteAsync(Id);
                var mainPage = Application.Current?.MainPage;
                if (mainPage != null && mainPage.Navigation != null)
                {

                    await mainPage.Navigation.PopAsync();
                }
            }
        }

    }
}
