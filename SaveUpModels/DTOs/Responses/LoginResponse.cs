using Newtonsoft.Json;
using SaveUpModels.DTOs;

namespace SaveUpModels.DTOs.Responses
{
    public class LoginResponse : UserResponse
    {
        [JsonProperty("auth")]
        public required TokenData Auth { get; set; }
    }
}
