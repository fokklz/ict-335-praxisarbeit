using Newtonsoft.Json;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUpModels.DTOs.Requests;
using System.Diagnostics;

namespace SaveUp.ViewModels
{
    public class AddViewModel : BaseNotifyHandler, IDisposable
    {
        private readonly IAlertService _alertService;
        private readonly IItemAPIService _itemAPIService;

        public string Currency { get; set; } = SettingsManager.Currency;

        public string Product { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public Command ClearCommand { get; }
        public Command AddCommand { get; }

        public AddViewModel(IAlertService alertService,IItemAPIService itemAPIService)
        {
            _alertService = alertService;
            _itemAPIService = itemAPIService;

            ClearCommand = new Command(Reset);
            AddCommand = new Command(async () => await Add());

            SettingsManager.PropertyChanged += SettingsManager_PropertyChanged;
        }

        public void Dispose()
        {
            SettingsManager.PropertyChanged -= SettingsManager_PropertyChanged;
        }

        private void SettingsManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsManager.Currency))
            {
                Currency = SettingsManager.Currency;
            }
        }

        public void Reset()
        {
            Product = string.Empty;
            Price = string.Empty;
            Description = string.Empty;
        }

        public async Task Add()
        {
            var price = 0;
            try {
                price = int.Parse(Price);
            }
            catch
            {
                await _alertService.ShowAsync("Invalid Price", "The price must be a number");
                return;
            }

            var item = new CreateItemRequest
            {
                Description = Description,
                Name = Product,
                Price = price,
                TimeSpan = SettingsManager.TimeSpan.ToString("dd-MM-yyyy.HH-mm-ss"),
                UserId = AuthManager.UserId!
            };

#if DEBUG
            Debug.WriteLine("Sending request: ", JsonConvert.SerializeObject(item));
#endif

            var response = await _itemAPIService.CreateAsync(item);
            if (response.IsSuccess)
            {
                // reload list
                var homeViewModel = App.ServiceProvider.GetService<HomeViewModel>()!;
                homeViewModel.LoadItems();

                // clear fields for next item
                Reset();
                await _alertService.ShowAsync(Localization.Instance.Dialog_CreatedItem_Title, Localization.Instance.Dialog_CreatedItem_Message);
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
