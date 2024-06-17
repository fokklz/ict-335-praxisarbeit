using SaveUpBackend.Common;
using SaveUpModels.DTOs;
using SaveUpModels.Models;

namespace SaveUpBackend.Interfaces
{
    public interface ITokenService
    {
        Task<TokenData> CreateToken(User user, bool keep = true);
        Task<TaskResult<RefreshResult>> RefreshToken(string token, string refreshToken);
    }
}