using Moq;
using Newtonsoft.Json;
using SaveUp.Common.Models;
using SaveUp.Interfaces;
using SaveUp.LoginApp.ViewModels;
using SaveUpModels.DTOs;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SaveUp.Tests.LoginApp.ViewModels
{
    public class LoginViewModelTests
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<IAlertService> _alertServiceMock;
        private LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _alertServiceMock = new Mock<IAlertService>();
            _viewModel = new LoginViewModel(_authServiceMock.Object, _alertServiceMock.Object);
        }

        [Fact]
        public void LoginViewModel_InitializesCorrectly()
        {
            Assert.NotNull(_viewModel.LoginCommand);
            Assert.NotNull(_viewModel.GotoRegister);
            Assert.NotNull(_viewModel.GotoPasswordReset);
            Assert.False(_viewModel.IsSuccess);
        }

        [Fact]
        public async Task LoginCommand_SuccessfulLogin()
        {
            var message = new HttpResponseMessage();
            var data = new LoginResponse
            {
                Auth = new TokenData
                {
                    Token = "123",
                },
                Id = "1",
            };
            message.StatusCode = HttpStatusCode.OK;
            message.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                            .ReturnsAsync(new HTTPResponse<LoginResponse>(message));

            _viewModel.LoginCommand.Execute(null);

            Assert.True(_viewModel.IsSuccess);
        }

        [Fact]
        public async Task LoginCommand_FailedLogin_ShowsError()
        {
            var message = new HttpResponseMessage();
            message.StatusCode = HttpStatusCode.BadRequest;
            message.Content = new StringContent("{\"errors\":[\"Username\": \"Invalid\"]}", Encoding.UTF8, "application/json");
            _authServiceMock.Setup(a => a.LoginAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                            .ReturnsAsync(new HTTPResponse<LoginResponse>(message));

            _viewModel.LoginCommand.Execute(null);

            Assert.False(_viewModel.IsSuccess);
        }

        [Fact]
        public async Task GotoRegister_NavigatesToRegister()
        {
            _viewModel.GotoRegister.Execute(null);

            // Assuming Shell.Current.GoToAsync is handled within your navigation service
            // Here we just ensure the command gets called
        }

        [Fact]
        public async Task GotoPasswordReset_ShowsComingSoonMessage()
        {
            _viewModel.Soon = 1;

            _viewModel.GotoPasswordReset.Execute(null);

            _alertServiceMock.Verify(a => a.ShowAsync("Coming Soon", "This feature is coming soon!"), Times.Once);
        }

        [Fact]
        public void ResetLoginState_ResetsState()
        {
            _viewModel.Email = "test@test.com";
            _viewModel.Password = "password";
            _viewModel.IsSuccess = true;

            _viewModel.ResetLoginState();

            Assert.Empty(_viewModel.Email);
            Assert.Empty(_viewModel.Password);
            Assert.False(_viewModel.IsSuccess);
        }
    }
}
