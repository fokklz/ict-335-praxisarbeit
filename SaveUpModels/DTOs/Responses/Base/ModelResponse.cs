using MongoDB.Bson;
using Newtonsoft.Json;
using SaveUpModels.Common.Attributes;
using SaveUpModels.Interfaces.Base;

namespace SaveUpModels.DTOs.Responses.Base
{
    public abstract class ModelResponse : IModel
    {
        [JsonProperty("id")]
        public required string Id { get; set; }
        ObjectId IModel.Id
        {
            get => ObjectId.Parse(Id);
            set => Id = value.ToString();
        }

        [AdminOnly]
        [JsonProperty("is_deleted")]
        public bool? IsDeleted { get; set; } = null;

        bool IModelBase.IsDeleted
        {
            get => IsDeleted ?? false;
            set => IsDeleted = value;
        }
    }
}
