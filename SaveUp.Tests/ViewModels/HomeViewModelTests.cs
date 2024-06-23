using Moq;
using Newtonsoft.Json;
using SaveUp.Common.Models;
using SaveUp.Interfaces.API;
using SaveUp.ViewModels;
using SaveUpModels.DTOs.Responses;
using System.Net;
using System.Text;

namespace SaveUp.Tests.ViewModels
{
    public class HomeViewModelTests
    {
        private readonly Mock<IItemAPIService> _itemAPIServiceMock;
        private readonly HomeViewModel _viewModel;

        public HomeViewModelTests()
        {
            _itemAPIServiceMock = new Mock<IItemAPIService>();
            _viewModel = new HomeViewModel(_itemAPIServiceMock.Object);
        }

        [Fact]
        public async Task LoadItems_CallsGetAllByUserIdAndTimeSpanAsync()
        {
            // Arrange
            var items = new List<ItemResponse>
            {
                new ItemResponse { Name = "Item1", Price = 10, Id = "1" },
                new ItemResponse { Name = "Item2", Price = 20, Id = "2" }
            };
            var message = new HttpResponseMessage();
            message.StatusCode = HttpStatusCode.OK;
            message.Content = new StringContent(JsonConvert.SerializeObject(items), Encoding.UTF8, "application/json");
            _itemAPIServiceMock.Setup(service => service.GetAllByUserIdAndTimeSpanAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new HTTPResponse<List<ItemResponse>>(message)).Verifiable();

            // Act
            await _viewModel.LoadItems();

            // Assert
            _itemAPIServiceMock.Verify(service => service.GetAllByUserIdAndTimeSpanAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            Assert.Equal(2, _viewModel.Items.Count);
            Assert.Equal(30, _viewModel.TotalAmount);
        }
    }
}
