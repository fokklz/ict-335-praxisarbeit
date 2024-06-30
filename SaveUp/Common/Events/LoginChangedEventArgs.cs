namespace SaveUp.Common.Events
{
    /// <summary>
    /// Event arguments for the login changed event
    /// </summary>
    public class LoginChangedEventArgs : EventArgs
    {
        public bool IsLoggedIn { get; set; }

        public string? Token { get; set; }

        public string? UserId { get; set; }

        public LoginChangedEventArgs()
        {
            IsLoggedIn = false;
            Token = null;
            UserId = null;
        }

        public LoginChangedEventArgs(string? token, string? userId)
        {
            IsLoggedIn = true;
            Token = token;
            UserId = userId;
        }
    }
}
