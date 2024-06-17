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
        public int? Price { get; set; }
    }
}
