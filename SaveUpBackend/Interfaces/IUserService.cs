using MongoDB.Bson;
using SaveUpBackend.Common;
using SaveUpBackend.Common.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;

namespace SaveUpBackend.Interfaces
{
    public interface IUserService : IBaseService<User, UserResponse, UpdateUserRequest, CreateUserRequest>
    {
        Task<TaskResult<object>> GetMe();
        Task<TaskResult<LoginResponse>> LoginAsync(LoginRequest model);
        Task<TaskResult<LoginResponse>> Refresh(RefreshRequest model);
        Task<TaskResult<object>> RevokeRefreshToken();
        Task<TaskResult<object>> UnlockAsync(string id);
        new Task<TaskResult<object>> CreateAsync(CreateUserRequest entity);
        new Task<TaskResult<object>> UpdateAsync(string id, UpdateUserRequest entity);
        Task<TaskResult<LoginResponse>> RegisterAsync(RegisterRequest createUserRequest);
    }
}