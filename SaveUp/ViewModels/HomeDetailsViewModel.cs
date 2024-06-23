using MongoDB.Bson;
using PropertyChanged;
using SaveUp.Common;
using SaveUp.Interfaces.API;

namespace SaveUp.ViewModels
{
    public class HomeDetailsViewModel : BaseNotifyHandler
    {
        [DependsOn(nameof(SettingsManager.Currency))]
        public string Currency => SettingsManager.Currency;

        public string Name { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime TimeSpan { get; set; }

        public string Id { get; set; }

        public Command DeleteCommand { get; }

        public HomeDetailsViewModel()
        {
            DeleteCommand = new Command(async () => await Delete());
        }

        public async Task Delete()
        {
           var result = await App.Current.MainPage.DisplayAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");
            if (result)
            {
                await App.ServiceProvider.GetService<IItemAPIService>()!.DeleteAsync(Id);
                await App.ServiceProvider.GetService<HomeViewModel>()!.LoadItems();
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }

    }
}
