using SaveUp.Common.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Interfaces.API
{
    public interface IItemAPIService : IBaseAPIService<CreateItemRequest, UpdateItemRequest, ItemResponse>
    {
    }
}
