using Moq;
using SaveUp.Common;
using SaveUp.Common.Models;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUp.Tests.Util.Helper;
using SaveUp.ViewModels;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using System.Net;

namespace SaveUp.Tests.ViewModels
{
    public class AddViewModelTests
    {
        private readonly Mock<IAlertService> _mockAlertService;
        private readonly Mock<IItemAPIService> _mockItemAPIService;
        private readonly AddViewModel _viewModel;

        public AddViewModelTests()
        {
            new Localization();
            SettingsManager.PreferencesAPI = new FakePreferences();
            SettingsManager.LoadSettings();

            _mockAlertService = new Mock<IAlertService>();
            _mockItemAPIService = new Mock<IItemAPIService>();
            _viewModel = new AddViewModel(_mockAlertService.Object, _mockItemAPIService.Object);
        }

        [Fact]
        public void Reset_ShouldClearAllFields()
        {
            // Arrange
            _viewModel.Product = "Test Product";
            _viewModel.Price = "100";
            _viewModel.Description = "Test Description";

            // Act
            _viewModel.Reset();

            // Assert
            Assert.Equal(string.Empty, _viewModel.Product);
            Assert.Equal(string.Empty, _viewModel.Price);
            Assert.Equal(string.Empty, _viewModel.Description);
        }

        [Fact]
        public async Task Add_InvalidPrice_ShouldShowAlert()
        {
            // Arrange
            _viewModel.Price = "invalid price";

            // Act
            await _viewModel.Add();

            // Assert
            _mockAlertService.Verify(a => a.ShowAsync("Invalid Price", "The price must be a number"), Times.Once);
        }

        [Fact]
        public async Task Add_ValidItem_ShouldResetFieldsAndShowAlert()
        {
            // Arrange
            _viewModel.Product = "Test Product";
            _viewModel.Price = "100";
            _viewModel.Description = "Test Description";

            _mockItemAPIService.Setup(api => api.CreateAsync(It.IsAny<CreateItemRequest>()))
                .ReturnsAsync(new HTTPResponse<ItemResponse>(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"id\":\"1\",\"name\":\"Test Product\",\"price\":100,\"description\":\"Test Description\"}", System.Text.Encoding.UTF8, "application/json"),
                }));

            // Act
            await _viewModel.Add();

            // Assert
            Assert.Equal(string.Empty, _viewModel.Product);
            Assert.Equal(string.Empty, _viewModel.Price);
            Assert.Equal(string.Empty, _viewModel.Description);
            _mockAlertService.Verify(a => a.ShowAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Add_ApiError_ShouldShowAlertWithErrorMessage()
        {
            // Arrange
            _viewModel.Product = "Test Product";
            _viewModel.Price = "100";
            _viewModel.Description = "Test Description";

            var errorResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"errors\":{\"error_key\":[\"error_message\"]}}", System.Text.Encoding.UTF8, "application/json"),
            };

            _mockItemAPIService.Setup(api => api.CreateAsync(It.IsAny<CreateItemRequest>()))
                .ReturnsAsync(new HTTPResponse<ItemResponse>(errorResponse));

            // Act
            await _viewModel.Add();

            // Assert
            _mockAlertService.Verify(a => a.ShowAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
