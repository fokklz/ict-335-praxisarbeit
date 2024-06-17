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
    public class UpdateUserRequest : UpdateRequest, IUser
    {
        [JsonProperty("role")]
        public RoleNames? Role { get; set; } = null;

        [JsonProperty("username")]
        [RegularExpression("^[a-zA-Z0-9._-]{3,}$", ErrorMessage = "Invalid username format.")]
        public string? Username { get; set; } = null;

        [JsonProperty("locked")]
        public bool? Locked { get; set; } = null;

        [JsonProperty("password")]
        public string? Password { get; set; } = null;

        // Implemented properties but with allowed null values

        string IUserBase.Username
        {
            get => Username ?? string.Empty;
            set => Username = value;
        }

        bool IUserBase.Locked
        {
            get => Locked ?? false;
            set => Locked = value;
        }

        RoleNames IUserBase.Role
        {
            get => Role ?? RoleNames.User;
            set => Role = value;
        }

        // Hidden properties since they are not allowed to be updated

        int IUserBase.LoginAttempts { get; set; }
        string? IUserBase.RefreshToken { get; set; }
        [AllowNull]
        byte[] IUserBase.PasswordHash { get; set; }
        [AllowNull]
        byte[] IUserBase.PasswordSalt { get; set; }
    }
}
