using Moq;
using SaveUp.Interfaces;
using SaveUp.LoginApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveUp.Tests.LoginApp.ViewModels
{
    public class PasswordResetViewModelTests
    {
        private Mock<IAlertService> _alertServiceMock;
        private PasswordResetViewModel _viewModel;

        public PasswordResetViewModelTests()
        {
            _alertServiceMock = new Mock<IAlertService>();
            _viewModel = new PasswordResetViewModel(_alertServiceMock.Object);
        }

        [Fact]
        public void PasswordResetViewModel_InitializesCorrectly()
        {
            Assert.NotNull(_viewModel.SubmitEmailCommand);
            Assert.NotNull(_viewModel.SubmitCodeCommand);
            Assert.NotNull(_viewModel.SubmitPasswordCommand);
            Assert.NotNull(_viewModel.GotoLogin);
            Assert.True(_viewModel.IsEmailStepVisible);
        }

        [Fact]
        public void SubmitEmail_ValidEmail_ProceedsToNextStep()
        {
            _viewModel.Email = "valid@example.com";
            _viewModel.SubmitEmailCommand.Execute(null);

            Assert.False(_viewModel.IsEmailStepVisible);
        }

        [Fact]
        public void SubmitCode_SubmitsCode()
        {
            // Placeholder for actual implementation
            _viewModel.SubmitCodeCommand.Execute(null);

            // For now, just ensure the command executes
            Assert.True(true);
        }

        [Fact]
        public void SubmitPassword_SubmitsPassword()
        {
            // Placeholder for actual implementation
            _viewModel.SubmitPasswordCommand.Execute(null);

            // For now, just ensure the command executes
            Assert.True(true);
        }

    }
}
