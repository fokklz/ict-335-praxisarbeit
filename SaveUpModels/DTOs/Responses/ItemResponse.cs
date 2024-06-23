using MongoDB.Bson;
using Newtonsoft.Json;
using SaveUpModels.Common.Attributes;
using SaveUpModels.DTOs.Responses.Base;
using SaveUpModels.Interfaces;
using SaveUpModels.Interfaces.Models;
using SaveUpModels.Models;
using System.Diagnostics.CodeAnalysis;

namespace SaveUpModels.DTOs.Responses
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    [ModelType(typeof(Item))]
    public class ItemResponse : ModelResponse, IItem, IResponseDTO
    {
        [AllowNull, NotNull]
        [JsonProperty("description")]
        public string Description { get; set; }

        [AllowNull, NotNull]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("timespan")]
        public string TimeSpan { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        ObjectId IItem.UserId
        {
            get => ObjectId.Parse(UserId);
            set => UserId = value.ToString();
        }

        [JsonIgnore]
        public DateTime CreatedAtLocal => CreatedAt.ToLocalTime();
    }
}
