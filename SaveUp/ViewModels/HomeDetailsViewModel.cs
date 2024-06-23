using MongoDB.Bson;
using PropertyChanged;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;

namespace SaveUp.ViewModels
{
    public class HomeDetailsViewModel : BaseNotifyHandler
    {
        private readonly IItemAPIService _itemAPIService;
        private readonly IAlertService _alertService;


        [DependsOn(nameof(SettingsManager.Currency))]
        public string Currency => SettingsManager.Currency;

        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime TimeSpan { get; set; }

        public string Id { get; set; }

        public Command DeleteCommand { get; }

        public HomeDetailsViewModel(IItemAPIService itemAPIService, IAlertService alertService)
        {
            _itemAPIService = itemAPIService;
            _alertService = alertService;

            DeleteCommand = new Command(async () => await Delete());
        }

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
