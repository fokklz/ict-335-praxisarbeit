using Newtonsoft.Json;
using SaveUpModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SaveUpModels.DTOs.Requests
{
    public class RefreshRequest : IAuthRequest
    {
        [Required]
        [JsonProperty("token")]
        public required string Token { get; set; }

        [Required]
        [JsonProperty("refresh_token")]
        public required string RefreshToken { get; set; }
    }
}
