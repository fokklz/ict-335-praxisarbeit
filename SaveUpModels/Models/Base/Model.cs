using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaveUpModels.Common.Attributes;
using SaveUpModels.Interfaces.Base;

namespace SaveUpModels.Models.Base
{
    public class Model : IModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("is_deleted")]
        [AdminOnly]
        public bool IsDeleted { get; set; } = false;
    }
}
