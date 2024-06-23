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

        public string Currency { get; set; } = SettingsManager.Currency;

        public int TotalAmount { get; set; } = 0;

        public string TimeSpan { get; set; } = string.Empty;

        public ObservableCollection<ItemResponse> Items { get; set; } = new ObservableCollection<ItemResponse>();

        private ItemResponse selectedItem;
        public ItemResponse SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem != value)
                {
                    //selectedItem = value;
                    //OnPropertyChanged(nameof(SelectedItem));
                    // TODO: open detail
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

                    App.Current?.MainPage?.Navigation.PushAsync(homeDetail);
                }
            }
        }

        public HomeViewModel(IItemAPIService itemAPIService)
        {
            _itemAPIService = itemAPIService;

            SettingsManager.PropertyChanged += async (s, e) => await SettingsManager_PropertyChanged(s, e);
        }

        public void Dispose()
        {
            SettingsManager.PropertyChanged -= async (s, e) => await SettingsManager_PropertyChanged(s, e);
        }

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
