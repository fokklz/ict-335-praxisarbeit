using SaveUp.Services;

namespace SaveUp.Tests.Services
{
    public class StorageServiceTests
    {
        private readonly StorageService _storageService;

        public StorageServiceTests()
        {
            _storageService = new StorageService();
        }

        [Fact]
        public void StoreUser_ShouldStoreUserCorrectly()
        {
            // Arrange
            string email = "test@example.com";
            string token = "test_token";
            string refreshToken = "test_refresh_token";

            // Act
            _storageService.StoreUser(email, token, refreshToken);

            // Assert
            Assert.Equal((token, refreshToken), _storageService.Get());
        }


        [Fact]
        public void Clear_ShouldClearUserCorrectly()
        {
            // Arrange
            _storageService.StoreUser("test@example.com", "test_token", "test_refresh_token");

            // Act
            _storageService.Clear();

            // Assert
            Assert.Equal((null, null), _storageService.Get());
        }
    }
}
