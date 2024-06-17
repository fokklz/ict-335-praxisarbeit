using Newtonsoft.Json;
using SaveUpModels.Common.Attributes;
using SaveUpModels.DTOs.Requests.Base;
using SaveUpModels.Enums;
using SaveUpModels.Interfaces;
using SaveUpModels.Interfaces.Models;
using SaveUpModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SaveUpModels.DTOs.Requests
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    [ModelType(typeof(User))]
    public class CreateUserRequest : CreateRequest, IUser
    {
        [JsonProperty("role")]
        public RoleNames Role { get; set; }

        [JsonProperty("username")]
        [RegularExpression("^[a-zA-Z0-9._-]{3,}$", ErrorMessage = "Invalid username format.")]
        public required string Username { get; set; }

        [JsonProperty("password")]
        public required string Password { get; set; }

        // Hidden properties since they are not allowed to be updated

        int IUserBase.LoginAttempts { get; set; }
        string? IUserBase.RefreshToken { get; set; }
        [AllowNull]
        byte[] IUserBase.PasswordHash { get; set; }
        [AllowNull]
        byte[] IUserBase.PasswordSalt { get; set; }
        bool IUserBase.Locked { get; set; }
    }
}
