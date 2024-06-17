using SaveUp.Common.Models;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Interfaces
{
    public interface IAuthService
    {
        Task<HTTPResponse<LoginResponse>> LoginAsync(string username, string password, bool rememberMe);
        Task<HTTPResponse<LoginResponse>> LoginAsyncWithToken(string token, string refreshToken);
        Task LogoutAsync(bool force = false);
    }
}