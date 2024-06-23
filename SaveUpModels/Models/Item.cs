using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaveUpModels.Interfaces;
using SaveUpModels.Models.Base;
using System.Diagnostics.CodeAnalysis;

namespace SaveUpModels.Models
{
    public class Item : Model, IItem
    {
        [BsonElement("description")]
        [AllowNull, NotNull]
        public string Description { get; set; }

        [BsonElement("name")]
        [AllowNull, NotNull]
        public string Name { get; set; }

        [BsonElement("price")]
        [AllowNull, NotNull]
        public int Price { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("timespan")]
        public string TimeSpan { get; set; }

        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonElement("user")]
        [AllowNull, NotNull]
        public virtual IUser User { get; set; }
        public bool ShouldSerializeUser() => false;
    }
}
