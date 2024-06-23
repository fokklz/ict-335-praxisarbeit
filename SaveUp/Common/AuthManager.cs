using SaveUp.Common.Events;

namespace SaveUp.Common
{
    /// <summary>
    /// Responsible for managing the authentication state of the application
    /// also allows for quick access to the current user id and token
    /// </summary>
    public static class AuthManager
    {

        /// <summary>
        /// Emitted when the login state for the application changes
        /// </summary>
        public static EventHandler<LoginChangedEventArgs> LoginChanged;

        /// <summary>
        /// Currently logged in state
        /// </summary>
        public static bool IsLoggedIn { get; private set; } = false;

        /// <summary>
        /// Token for the current user
        /// </summary>
        public static string? Token { get; private set; } = null;

        /// <summary>
        /// Refresh token for the current user
        /// manly needed to remove a stored user
        /// </summary>
        public static string? RefreshToken { get; private set; } = null;

        /// <summary>
        /// The id of the current user
        /// </summary>
        public static string? UserId { get; private set; } = null;

        /// <summary>
        /// Login a user to the application
        /// </summary>
        /// <param name="token">The Token to use for API Requests</param>
        /// <param name="refreshToken">The refreshToken for the user</param>
        /// <param name="userId">THe userId of the user</param>
        public static void Login(string token, string? refreshToken, string userId)
        {
            RefreshToken = refreshToken;
            Token = token;
            UserId = userId;
            IsLoggedIn = true;
            LoginChanged?.Invoke(null, new LoginChangedEventArgs(token, userId));
        }

        /// <summary>
        /// Logout the user from the application
        /// </summary>
        public static void Logout()
        {
            RefreshToken = null;
            Token = null;
            UserId = null;
            IsLoggedIn = false;
            LoginChanged?.Invoke(null, new LoginChangedEventArgs());
        }

    }
}
