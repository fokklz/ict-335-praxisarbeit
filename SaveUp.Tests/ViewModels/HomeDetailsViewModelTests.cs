using Moq;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUp.Tests.Util.Helper;
using SaveUp.ViewModels;
using SaveUp.Views;

namespace SaveUp.Tests.ViewModels
{
    public class FakeHome
    {
        public string Title { get; set; }
    }

    public class HomeDetailsViewModelTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IStorageService> _storageServiceMock;

        private readonly Mock<IItemAPIService> _itemAPIServiceMock;
        private readonly Mock<IAlertService> _alertServiceMock;

        private readonly HomeDetailsViewModel _viewModel;


        public HomeDetailsViewModelTests()
        {
            new Localization();
            SettingsManager.PreferencesAPI = new FakePreferences();
            SettingsManager.LoadSettings();

            _serviceProviderMock = new Mock<IServiceProvider>();

            _itemAPIServiceMock = new Mock<IItemAPIService>();
            _alertServiceMock = new Mock<IAlertService>();
            _alertServiceMock.Setup(service => service.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _viewModel = new HomeDetailsViewModel(_itemAPIServiceMock.Object, _alertServiceMock.Object);

            _serviceProviderMock.Setup(provider => provider.GetService(typeof(HomeDetailsViewModel)))
                .Returns(_viewModel);
            _serviceProviderMock.Setup(provider => provider.GetService(typeof(HomeDetailsPage)))
                .Returns(new FakeHome());

            App.ServiceProvider = _serviceProviderMock.Object;
        }

        [Fact]
        public async Task DeleteCommand_ExecutesDelete()
        {
            // Arrange
            var itemId = "item123";
            _viewModel.Id = itemId;

            // Act
            _viewModel.DeleteCommand.Execute(null);

            // Assert
            _itemAPIServiceMock.Verify(service => service.DeleteAsync(itemId), Times.Once);
        }

        [Fact]
        public void Currency_PropertyChanged()
        {
            // Arrange
            var expectedCurrency = "USD";

            // Act
            SettingsManager.SetCurrency(expectedCurrency);

            // Assert
            Assert.Equal(expectedCurrency, _viewModel.Currency);
        }
    }
}
