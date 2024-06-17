using MongoDB.Bson;
using System.Diagnostics.CodeAnalysis;

namespace SaveUpModels.DTOs.Responses
{
    public class DeleteResponse
    {
        [AllowNull, NotNull]
        public string Id { get; set; }
    }
}
