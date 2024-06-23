using SaveUp.Common.Interfaces;
using SaveUp.Common.Models;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Interfaces.API
{
    public interface IItemAPIService : IBaseAPIService<CreateItemRequest, UpdateItemRequest, ItemResponse>
    {
        Task<HTTPResponse<List<ItemResponse>>> GetAllByUserIdAndTimeSpanAsync(string userId, DateTime timeSpan);
    }
}
