using SaveUp.Common.Interfaces;
using SaveUp.Common.Models;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Interfaces.API
{
    public interface IUserAPIService : IBaseAPIService<CreateUserRequest, UpdateUserRequest, UserResponse>
    {
        Task<HTTPResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<HTTPResponse<LoginResponse>> RefreshAsync(RefreshRequest request);
        Task<HTTPResponse<UserResponse>> RevokeAsync();
        Task<HTTPResponse<UserResponse>> UnlockUser(int userId);
    }
}