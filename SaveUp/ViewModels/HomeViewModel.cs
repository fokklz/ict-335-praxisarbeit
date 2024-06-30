using PropertyChanged;
using SaveUp.Common;
using SaveUp.Interfaces.API;
using SaveUp.Views;
using SaveUpModels.DTOs.Responses;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace SaveUp.ViewModels
{
    public class HomeViewModel : BaseNotifyHandler, IDisposable
    {
        private readonly IItemAPIService _itemAPIService;

        /// <summary>
        /// Currency to use
        /// </summary>
        public string Currency { get; set; } = SettingsManager.Currency;

        /// <summary>
        /// The total amount of all items
        /// </summary>
        public int TotalAmount { get; set; } = 0;

        /// <summary>
        /// The items to display
        /// </summary>
        public ObservableCollection<ItemResponse> Items { get; set; } = new ObservableCollection<ItemResponse>();

        /// <summary>
        /// The selected item, to open the detail page
        /// </summary>
        private ItemResponse selectedItem;
        public ItemResponse SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    var homeDetail = App.ServiceProvider.GetService<HomeDetailsPage>()!;
                    homeDetail.Title = value.Name;

                    var homeDetailViewModel = App.ServiceProvider.GetService<HomeDetailsViewModel>()!;
                    homeDetailViewModel.Name = value.Name;
                    homeDetailViewModel.Description = value.Description;
                    homeDetailViewModel.Price = value.Price;
                    homeDetailViewModel.CreatedAt = value.CreatedAt;
                    homeDetailViewModel.Id = value.Id;

                    DateTime parsedDate;
                    if (DateTime.TryParseExact(value.TimeSpan, "dd-MM-yyyy.HH-mm-ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    {
                        homeDetailViewModel.TimeSpan = parsedDate;
                    }

                    var mainPage = Application.Current?.MainPage;
                    if (mainPage != null && mainPage.Navigation != null)
                    {
                        mainPage.Navigation.PushAsync(homeDetail);
                    }
                }
            }
        }

        public HomeViewModel(IItemAPIService itemAPIService)
        {
            _itemAPIService = itemAPIService;

            SettingsManager.PropertyChanged += async (s, e) => await SettingsManager_PropertyChanged(s, e);
        }

        /// <summary>
        /// Dispose the view model
        /// </summary>
        public void Dispose()
        {
            SettingsManager.PropertyChanged -= async (s, e) => await SettingsManager_PropertyChanged(s, e);
        }

        /// <summary>
        /// Subscribe to the property changed event of the settings manager to reflect changes in the currency and reload the items on time span change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task SettingsManager_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsManager.TimeSpan))
            {
                await LoadItems();
            }else if (e.PropertyName == nameof(SettingsManager.Currency))
            {
                Currency = SettingsManager.Currency;
            }
        }

        /// <summary>
        /// Clear the items and reload them also calculate the total amount
        /// </summary>
        /// <returns></returns>
        public async Task LoadItems()
        {
            var res = await _itemAPIService.GetAllByUserIdAndTimeSpanAsync(AuthManager.UserId!, SettingsManager.TimeSpan);
            if (res.IsSuccess)
            {
                TotalAmount = 0;
                Items.Clear();
                foreach (var item in await res.ParseSuccess() ?? [])
                {
                    TotalAmount += item.Price;
                    Items.Add(item);
                }
            }
        }

        


    }
}
