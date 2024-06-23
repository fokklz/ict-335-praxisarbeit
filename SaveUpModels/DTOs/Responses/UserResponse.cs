using Newtonsoft.Json;
using SaveUpModels.Common.Attributes;
using SaveUpModels.DTOs.Responses.Base;
using SaveUpModels.Enums;
using SaveUpModels.Interfaces;
using SaveUpModels.Interfaces.Models;
using SaveUpModels.Models;
using System.Diagnostics.CodeAnalysis;

namespace SaveUpModels.DTOs.Responses
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    [ModelType(typeof(User))]
    public class UserResponse : ModelResponse, IUser, IResponseDTO
    {
        [AllowNull, NotNull]
        [JsonProperty("email")]
        public string Email { get; set; }

        [AllowNull, NotNull]
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("locked")]
        public bool? Locked { get; set; }

        [JsonProperty("role")]
        public string? Role { get; set; }

        // Specially implemented properties to allow for null values and parsing

        bool IUserBase.Locked
        {
            get => Locked ?? false;
            set => Locked = value;
        }

        RoleNames IUserBase.Role
        {
            get => Role == null ? RoleNames.User : (RoleNames)Enum.Parse(typeof(RoleNames), Role);
            set => Role = value.ToString();
        }

        // Hidden properties since they are not allowed to be displayed

        [AllowNull]
        byte[] IUserBase.PasswordHash { get; set; }
        [AllowNull]
        byte[] IUserBase.PasswordSalt { get; set; }
        string? IUserBase.RefreshToken { get; set; }
        int IUserBase.LoginAttempts { get; set; }
    }
}
