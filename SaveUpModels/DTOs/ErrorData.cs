using Newtonsoft.Json;
using SaveUpModels.Interfaces;
using System.Collections.Generic;

namespace SaveUpModels.DTOs
{
    public class ErrorData : IDTO
    {
        [JsonProperty("code")]
        public string? Code { get; set; } = null;

        [JsonProperty("message")]
        public string? Message { get; set; } = null;

        [JsonProperty("errors")]
        public Dictionary<string, List<string>>? Errors { get; set; } = null;

        [JsonProperty("type")]
        public string? Type { get; set; } = null;

        [JsonProperty("title")]
        public string? Title { get; set; } = null;

        [JsonProperty("status")]
        public int? Status { get; set; } = null;
    }
}
