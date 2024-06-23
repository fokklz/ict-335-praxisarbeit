using Moq;
using SaveUp.Common;
using SaveUp.Interfaces;
using SaveUp.Tests.Util.Helper;
using SaveUp.ViewModels;

namespace SaveUp.Tests.ViewModels
{
    public class SettingsViewModelTests
    {
        private readonly Mock<IAlertService> _alertServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly SettingsViewModel _viewModel;

        public SettingsViewModelTests()
        {
            new Localization();
            SettingsManager.PreferencesAPI = new FakePreferences();
            SettingsManager.LoadSettings();

            _alertServiceMock = new Mock<IAlertService>();
            _authServiceMock = new Mock<IAuthService>();
            _viewModel = new SettingsViewModel(_alertServiceMock.Object, _authServiceMock.Object);
        }

        [Fact]
        public void Should_Initialize_SettingsViewModel()
        {
            Assert.NotNull(_viewModel);
        }

        [Fact]
        public void Should_Change_Language()
        {
            // Arrange
            var newLanguage = "English";

            // Act
            _viewModel.SelectedLanguage = newLanguage;

            // Assert
            Assert.Equal(newLanguage, _viewModel.SelectedLanguage);
        }

        [Fact]
        public void Should_Change_Theme()
        {
            // Arrange
            var newTheme = Localization.Instance.GetThemeDropdown()[0];

            // Act
            _viewModel.SelectedTheme = newTheme;

            // Assert
            Assert.Equal(newTheme, _viewModel.SelectedTheme);
        }

        [Fact]
        public void Should_Change_Currency()
        {
            // Arrange
            var newCurrency = "USD";

            // Act
            _viewModel.Currency = newCurrency;

            // Assert
            Assert.Equal(newCurrency, _viewModel.Currency);
        }

        [Fact]
        public async Task Should_Logout()
        {
            // Arrange
            _authServiceMock.Setup(service => service.LogoutAsync(It.IsAny<bool>())).Returns(Task.CompletedTask);

            // Act
            _viewModel.LogoutCommand.Execute(null);

            // Assert
            _authServiceMock.Verify(service => service.LogoutAsync(It.IsAny<bool>()), Times.Once);
        }
    }
}
