using Newtonsoft.Json;
using SaveUpModels.Common.Attributes;
using SaveUpModels.DTOs.Responses.Base;
using SaveUpModels.Interfaces;
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
        public int? Price { get; set; }
    }
}
