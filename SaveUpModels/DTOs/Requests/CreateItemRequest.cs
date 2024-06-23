using MongoDB.Bson;
using Newtonsoft.Json;
using SaveUpModels.Common.Attributes;
using SaveUpModels.DTOs.Requests.Base;
using SaveUpModels.Interfaces;
using SaveUpModels.Interfaces.Models;
using SaveUpModels.Models;
using System.ComponentModel.DataAnnotations;

namespace SaveUpModels.DTOs.Requests
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    [ModelType(typeof(Item))]
    public class CreateItemRequest : CreateRequest, IItem
    {

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;


        [JsonProperty("name")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public required string Name { get; set; }


        [JsonProperty("price")]
        [Range(1, 1000, ErrorMessage = "The price must be between 1 and 1000.")]
        public int Price { get; set; }

        ObjectId IItem.UserId
        {
            get => ObjectId.Parse(UserId);
            set => UserId = value.ToString();
        }

        [JsonProperty("timespan")]
        public string TimeSpan { get; set; }
    }
}
