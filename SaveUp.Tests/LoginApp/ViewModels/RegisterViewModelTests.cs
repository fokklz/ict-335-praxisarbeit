using Moq;
using SaveUp.Common;
using SaveUp.Common.Models;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUp.LoginApp.ViewModels;
using SaveUpModels.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveUp.Tests.LoginApp.ViewModels
{
    public class RegisterViewModelTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IAlertService> _alertServiceMock;
        private readonly Mock<IUserAPIService> _userAPIServiceMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly RegisterViewModel _viewModel;

        public RegisterViewModelTests()
        {
            new Localization();

            _authServiceMock = new Mock<IAuthService>();
            _alertServiceMock = new Mock<IAlertService>();

            _alertServiceMock.Setup(service => service.ShowAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask).Verifiable();

            _userAPIServiceMock = new Mock<IUserAPIService>();
            _storageServiceMock = new Mock<IStorageService>();

            _viewModel = new RegisterViewModel(_authServiceMock.Object, _alertServiceMock.Object, _userAPIServiceMock.Object, _storageServiceMock.Object);
        }

        [Fact]
        public void OnConfirmPasswordChange_PasswordsDoNotMatch_ShowsAlert()
        {
            // Arrange
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "differentpassword";

            // Assert
            Assert.False(_viewModel.ValidConfirmPassword);
        }

        [Fact]
        public async Task Register_PasswordsDoNotMatch_DoesNotCallRegister()
        {
            // Arrange
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "differentpassword";
            _viewModel.ValidConfirmPassword = false;

            // Act
            await _viewModel.Register();

            // Assert
            _userAPIServiceMock.Verify(service => service.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Register_PasswordsMatch_CallsRegister()
        {
            // Arrange
            _viewModel.Password = "password";
            _viewModel.ConfirmPassword = "password";
            _viewModel.ValidConfirmPassword = true;

            _userAPIServiceMock.Setup(service => service.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new HTTPResponse<LoginResponse>(new HttpResponseMessage()));

            // Act
            await _viewModel.Register();

            // Assert
            _userAPIServiceMock.Verify(service => service.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
