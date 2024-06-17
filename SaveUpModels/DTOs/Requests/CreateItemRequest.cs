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

        [JsonProperty("description")]
        [MinLength(20, ErrorMessage = "The description must at least 20 characters long.")]
        public required string Description { get; set; }


        [JsonProperty("name")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        public required string Name { get; set; }


        [JsonProperty("price")]
        [Range(1, 1000, ErrorMessage = "The price must be between 1 and 1000.")]
        public int Price { get; set; }

        int? IItemBase.Price
        {
            get => Price;
            set => Price = value ?? 0;
        }
    }
}
