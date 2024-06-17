using Microsoft.Extensions.Configuration;
using SaveUp.Common;
using SaveUp.Common.Models;
using SaveUp.Interfaces.API;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Services.API
{
    public class UserAPIService : BaseAPIService<CreateUserRequest, UpdateUserRequest, UserResponse>, IUserAPIService
    {
        public UserAPIService(IConfiguration configuration) : base(configuration, "users")
        {
        }

        /// <summary>
        /// Login a user to the application 
        /// - The reveived token is stored in the storage service
        /// </summary>
        /// <param name="request">The login information to use</param>
        /// <returns>The login response</returns>
        public async Task<HTTPResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var res = await _sendRequest(HttpMethod.Post, _url("login"), request);
            return new HTTPResponse<LoginResponse>(res);
        }

        /// <summary>
        /// Refresh a user's token
        /// </summary>
        /// <param name="request">The refresh-login information to use</param>
        /// <returns>The login response</returns>
        public async Task<HTTPResponse<LoginResponse>> RefreshAsync(RefreshRequest request)
        {
            var res = await _sendRequest(HttpMethod.Post, _url("refresh"), request);
            return new HTTPResponse<LoginResponse>(res);
        }

        /// <summary>
        /// Revoke a user's token (refresh-token)
        /// </summary>
        /// <returns>THe user that just got logged out</returns>
        public async Task<HTTPResponse<UserResponse>> RevokeAsync()
        {
            var res = await _sendRequest(HttpMethod.Post, _url("revoke"));
            return new HTTPResponse<UserResponse>(res);
        }

        /// <summary>
        /// Unlock a user
        /// </summary>
        /// <param name="userId">The user to unlock</param>
        /// <returns>The Information of the unlocked user</returns>
        public async Task<HTTPResponse<UserResponse>> UnlockUser(int userId)
        {
            var res = await _sendRequest(HttpMethod.Post, _url(userId.ToString(), "unlock"));
            return new HTTPResponse<UserResponse>(res);
        }
    }
}
