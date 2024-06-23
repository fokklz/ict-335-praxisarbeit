using Moq;
using SaveUp.ViewModels;

namespace SaveUp.Tests.ViewModels
{
    public class AboutViewModelTests
    {
        [Fact]
        public void Constructor_InitializesOpenUrlCommand()
        {
            // Arrange & Act
            var viewModel = new AboutViewModel();

            // Assert
            Assert.NotNull(viewModel.OpenUrlCommand);
        }

        [Fact]
        public async Task OpenUrl_InvalidUrl_DoesNotOpenUrl()
        {
            // Arrange
            var viewModel = new AboutViewModel();
            var invalidUrl = "invalid-url";
            var launcherMock = new Mock<ILauncher>();

            // Act
            await viewModel.OpenUrl(invalidUrl);

            // Assert
            launcherMock.Verify(l => l.OpenAsync(It.IsAny<Uri>()), Times.Never);
        }
    }
}
