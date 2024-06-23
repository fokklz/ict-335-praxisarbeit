using PropertyChanged;
using SaveUp.Common;
using SaveUp.Common.Models;
using SaveUp.Interfaces;
using SaveUp.Interfaces.API;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using System.Diagnostics;

namespace SaveUp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAPIService _userAPIService;
        private readonly IStorageService _storageService;
        private readonly IAlertService _alertService;

        public AuthService(IUserAPIService userAPIService, IStorageService storageService, IAlertService alertService)
        {
            _storageService = storageService;
            _userAPIService = userAPIService;
            _alertService = alertService;
        }

        /// <summary>
        /// Internal method to handle the login response
        /// Since both Login and Refresh use the same logic, we can use this method for both
        /// </summary>
        /// <param name="res">The response obtained by Login or Refresh</param>
        /// <param name="oldRefreshToken">used to remove this refresh token if the response turns out to be falsy</param>
        /// <returns>The Response, The command to switch to main app</returns>
        private async Task<HTTPResponse<LoginResponse>> _handleLoginResponse(HTTPResponse<LoginResponse> res, string oldRefreshToken = null)
        {
            if (res.IsSuccess)
            {
                // we directly process the token here, since we need to store it in the storage service
                var parsed = await res.ParseSuccess();
                if (parsed != null && parsed.Auth != null && parsed.Auth.Token != null)
                {
                    var refreshToken = parsed.Auth.RefreshToken;
                    if (refreshToken != null)
                    {
                        _storageService.StoreUser(parsed.Email, parsed.Auth.Token, refreshToken);
                        await _storageService.SaveChangesAsync();
                    }

                    AuthManager.Login(parsed.Auth.Token, refreshToken, parsed.Id);

                    return res;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(oldRefreshToken))
                {
                    _storageService.Clear();
                    await _storageService.SaveChangesAsync();
                }
            }

            return res;
        }

        /// <summary>
        /// Try to login a user from the storage
        /// </summary>
        public async Task TryLoginFromStorage()
        {

            if (_storageService.HasUser())
            {
                var user = _storageService.Get(); 
                var res = await LoginAsyncWithToken(user.Item1, user.Item2);
                Debug.WriteLine($"Login from storage: {res.IsSuccess}");
                if (res.IsSuccess)
                {
                    Debug.WriteLine("--- Login from storage successful");
                }
            }
        }

        /// <summary>
        /// Login a user to the application using a token and a refresh token
        /// </summary>
        /// <param name="token">The token of the user</param>
        /// <param name="refreshToken">The refresh token of the user</param>
        /// <returns>The Response, The command to switch to main app</returns>
        public async Task<HTTPResponse<LoginResponse>> LoginAsyncWithToken(string token, string refreshToken)
        {
            var data = new RefreshRequest
            {
                Token = token,
                RefreshToken = refreshToken
            };
            var res = await _userAPIService.RefreshAsync(data);
            return await _handleLoginResponse(res, refreshToken);
        }

        /// <summary>
        /// Login a user to the application
        /// </summary>
        /// <param name="email">The username of the user</param>
        /// <param name="password">The password of the user</param>
        /// <param name="rememberMe">Whether the user should be remembered for future logins</param>
        /// <returns>The Response, The command to switch to main app</returns>
        public async Task<HTTPResponse<LoginResponse>> LoginAsync(string email, string password, bool rememberMe)
        {
            var data = new LoginRequest
            {
                Email = email,
                Password = password,
                RememberMe = rememberMe
            };
            var res = await _userAPIService.LoginAsync(data);
            return await _handleLoginResponse(res);
        }

        /// <summary>
        /// Logout a user from the application
        /// </summary>
        /// <returns>a ICommand to be run when ready to navigate away</returns>
        public async Task LogoutAsync(bool force = false)
        {
            var result = false;
            if (!force) result = await _alertService.ConfirmAsync("LogoutTitle", "LogoutMessage");
            if (force || result)
            {
                _storageService.Clear();
                AuthManager.Logout();
            }
        }
    }
} 
