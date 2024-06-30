using Microsoft.Extensions.Configuration;
using SaveUp.Common;
using SaveUp.Common.Models;
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

        /// <summary>
        /// Get all items by user id and time span
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="timeSpan">The time span</param>
        /// <returns>The response wrapped in a HTTPResponse</returns>
        public async Task<HTTPResponse<List<ItemResponse>>> GetAllByUserIdAndTimeSpanAsync(string userId, DateTime timeSpan)
        {
            var res = await _sendRequest(HttpMethod.Get, _url("user", userId.ToString(), timeSpan.ToString("dd-MM-yyyy.HH-mm-ss")));
            return new HTTPResponse<List<ItemResponse>>(res);
        }
    }
}
