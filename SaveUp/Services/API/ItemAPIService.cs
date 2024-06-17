using Microsoft.Extensions.Configuration;
using SaveUp.Common;
using SaveUp.Interfaces.API;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Services.API
{
    public class ItemAPIService : BaseAPIService<CreateItemRequest, UpdateItemRequest, ItemResponse>, IItemAPIService
    {
        public ItemAPIService(IConfiguration configuration) : base(configuration, "items")
        {
        }
    }
}
