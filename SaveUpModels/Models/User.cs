using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaveUpModels.Common.Attributes;
using SaveUpModels.Enums;
using SaveUpModels.Interfaces;
using SaveUpModels.Models.Base;
using System.Diagnostics.CodeAnalysis;

namespace SaveUpModels.Models
{
    public class User : Model, IUser
    {

        [BsonElement("username")]
        [AllowNull, NotNull]
        public string Username { get; set; }

        [BsonElement("password_hash")]
        [BsonRepresentation(BsonType.Binary)]
        [AllowNull, NotNull]
        public byte[] PasswordHash { get; set; }

        [BsonElement("password_salt")]
        [BsonRepresentation(BsonType.Binary)]
        [AllowNull, NotNull]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("locked")]
        [AdminOnly]
        public bool Locked { get; set; } = false;

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        [OwnerOrAdminOnly]
        public RoleNames Role { get; set; } = RoleNames.User;

        [BsonElement("login_attempts")]
        public int LoginAttempts { get; set; } = 0;

        [BsonElement("refresh_token")]
        public string? RefreshToken { get; set; } = null;
    }
}
