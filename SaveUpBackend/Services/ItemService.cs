using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SaveUpBackend.Common;
using SaveUpBackend.Common.Generics;
using SaveUpBackend.Interfaces;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Models;
using StackExchange.Redis;

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

        /// <summary>
        /// Get all items by timespan
        /// </summary>
        /// <param name="timeSpan">The timespan start</param>
        /// <returns>The matching items as IEnumerable</returns>
        public async Task<TaskResult<IEnumerable<object>>> GetByTimeSpanAsync(string timeSpan)
        {
            var query = await _context.Orders.FindAsync(Builders<Item>.Filter.Eq(e => e.TimeSpan, timeSpan));
            var filtered = ApplyFilter(query.AsQueryable());
            var orders = filtered.ToList();

            return ResolveList(orders);
        }

        /// <summary>
        /// Get all items by timespan
        /// </summary>
        /// <param name="timeSpan">The timespan start</param>
        /// <returns>The matching items as IEnumerable</returns>
        public async Task<TaskResult<IEnumerable<object>>> GetByTimeSpanForUserAsync(string timeSpan, string userId)
        {
            var query = await _context.Orders.FindAsync(Builders<Item>.Filter.And(
                Builders<Item>.Filter.Eq(e => e.TimeSpan, timeSpan),
                Builders<Item>.Filter.Eq(e => e.UserId, ObjectId.Parse(userId))
            ));
            var filtered = ApplyFilter(query.AsQueryable());
            var orders = filtered.ToList();

            return ResolveList(orders);
        }
    }
}