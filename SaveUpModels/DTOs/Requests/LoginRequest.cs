using Newtonsoft.Json;
using SaveUpModels.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SaveUpModels.DTOs.Requests
{
    public class LoginRequest : IAuthRequest
    {
        [Required]
        [JsonProperty("email")]
        public required string Email { get; set; }

        [Required]
        [JsonProperty("password")]
        public required string Password { get; set; }

        [JsonProperty("remember_me")]
        public bool RememberMe { get; set; } = false;
    }
}
