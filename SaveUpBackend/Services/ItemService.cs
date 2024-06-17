using AutoMapper;
using SaveUpBackend.Common.Generics;
using SaveUpBackend.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;

namespace SaveUpBackend.Services
{
    public class ItemService : GenericService<Item, ItemResponse, UpdateItemRequest, CreateItemRequest>, IItemService
    {
        private readonly IMongoDBContext _context;
        private readonly IMapper _mapper;

        public ItemService(IMongoDBContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
        }



    }
}