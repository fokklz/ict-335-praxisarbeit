using SaveUpModels.Enums;
using SaveUpModels.Interfaces.Base;

namespace SaveUpModels.Interfaces.Models
{
    public interface IUserBase : IModelBase
    {
        bool Locked { get; set; }
        int LoginAttempts { get; set; }
        byte[] PasswordHash { get; set; }
        byte[] PasswordSalt { get; set; }
        string? RefreshToken { get; set; }
        RoleNames Role { get; set; }
        string Username { get; set; }
    }
}