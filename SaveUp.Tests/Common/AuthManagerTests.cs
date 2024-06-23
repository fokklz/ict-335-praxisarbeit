using SaveUp.Common;

namespace SaveUp.Tests.Common
{
    public class AuthManagerTests
    {
        [Fact]
        public async Task SetPropertiesCorrectlyOnLogin()
        {
            var token = "123";
            var refreshToken = "1234";
            var userId = "1";

            AuthManager.Login(token, refreshToken, userId);

            Assert.Equal(token, AuthManager.Token);
            Assert.Equal(refreshToken, AuthManager.RefreshToken);
            Assert.Equal(userId, AuthManager.UserId);
            Assert.True(AuthManager.IsLoggedIn);
        }

        [Fact]
        public async Task RemovePropertiesCorrectlyOnLogout()
        {
            AuthManager.Logout();

            Assert.Null(AuthManager.Token);
            Assert.Null(AuthManager.RefreshToken);
            Assert.Null(AuthManager.UserId);
            Assert.False(AuthManager.IsLoggedIn);
        }

        [Fact]
        public async Task LoginChanged_CalledOnLogin()
        {
            var eventCalled = 0;
            AuthManager.LoginChanged += (sender, args) => { eventCalled += 1; };

            AuthManager.Login("123", null, "1");

            Assert.Equal(1, eventCalled);
        }

        [Fact]
        public async Task LoginChanged_CalledOnLogout()
        {
            var eventCalled = 0;
            AuthManager.LoginChanged += (sender, args) => { eventCalled += 1; };

            AuthManager.Logout();

            Assert.Equal(1, eventCalled);
        }
    }
}
