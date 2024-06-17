using SaveUpBackend.Common.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;

namespace SaveUpBackend.Interfaces
{
    public interface IItemService : IBaseService<Item, ItemResponse, UpdateItemRequest,CreateItemRequest>
    {
    }
}