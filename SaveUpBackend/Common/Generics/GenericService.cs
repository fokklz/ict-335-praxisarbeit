using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SaveUpBackend.Common.Enums;
using SaveUpBackend.Common.Interfaces;
using SaveUpBackend.Interfaces;
using SaveUpModels.DTOs.Responses;
using SaveUpModels.Enums;
using SaveUpModels.Interfaces.Base;
using System.Security.Claims;

namespace SaveUpBackend.Common.Generics
{
    public class GenericService<T, TResponse, TUpdate, TCreate> : IBaseService<T, TResponse, TUpdate, TCreate>
        where T : class, IModel
        where TResponse : class
        where TUpdate : class
        where TCreate : class
    {
        private readonly IMongoDBContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenericService(IMongoDBContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Check if the current user is an Admin
        /// </summary>
        /// <returns>true if admin else false</returns>
        protected bool IsAdmin()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.IsInRole(RoleNames.SuperAdmin.ToString()) ?? false;
        }


        protected FilterDefinition<T> GetFilter(ObjectId id)
        {
            return Builders<T>.Filter.Eq("_id", id);
        }

        protected FilterDefinition<T> GetFilter(string id)
        {
            return GetFilter(ObjectId.Parse(id));
        }

        /// <summary>
        /// Apply the filter to the query to hide soft-deleted entries from non admin users
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="query">The query to apply the filter on</param>
        /// <returns>the filtered content</returns>
        protected IQueryable<T> ApplyFilter(IQueryable<T> query)
        {
            bool isAdmin = IsAdmin();
            return query.Where(e => isAdmin || e.IsDeleted == false);
        }

        /// <summary>
        /// Helper to distinguish between admin and non admin responses for a single entity
        /// </summary>
        /// <typeparam name="TModel">T</typeparam>
        /// <param name="data">the data to map</param>
        /// <returns>the mapped data in the coresponding DTO</returns>
        protected TaskResult<object> Resolve<TModel>(TModel data)
            where TModel : class, IModel
        {
            return CreateTaskResult.Success<object>(_mapper.Map<TResponse>(data, opts =>
            {
                opts.Items["IsAdmin"] = IsAdmin();
                opts.Items["IsOwner"] = IsOwnerOrAdmin(data);
            }));
        }

        /// <summary>
        /// Helper to distinguish between admin and non admin responses for a list-like entity
        /// </summary>
        /// <typeparam name="TModel">T</typeparam>
        /// <param name="data">the data to map</param>
        /// <returns>the mapped data in the coresponding DTO</returns>
        protected TaskResult<IEnumerable<object>> ResolveList<TModel>(IEnumerable<TModel> data)
            where TModel : class, IModel
        {
            return CreateTaskResult.Success<IEnumerable<object>>(_mapper.Map<IEnumerable<TResponse>>(data, opts =>
            {
                opts.Items["IsAdmin"] = IsAdmin();
                opts.Items["IsOwner"] = false;
            }));
        }


        /// <summary>
        /// Wrapper for endpoints restricted to the owner of the entity (e.g. delete) beeing assigned or superadmin will pass
        /// </summary>
        /// <param name="id">The id of the entity in the database to check</param>
        /// <param name="allowAdmin">set to false to disallow admins</param>
        /// <returns>The found entity from the database if allowed else null</returns>
        public bool IsOwnerOrAdmin<TModel>(TModel? item, bool allowAdmin = true)
            where TModel : class, IModel
        {
            if (item == null) return false;
            var user = _httpContextAccessor.HttpContext?.User;
            var isAdmin = IsAdmin();
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var owenerId = (item?.GetType().GetProperty("UserId")?.GetValue(item, null) ?? item?.Id)?.ToString();

            if (isAdmin && allowAdmin || !string.IsNullOrEmpty(owenerId) && userId == owenerId)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsOwnerOrAdmin(string id, bool allowAdmin = true)
        {
            var item = (await _context.Set<T>().FindAsync(GetFilter(id))).FirstOrDefault();
            return IsOwnerOrAdmin(item, allowAdmin);
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>TResponseBase as TaskResult</returns>
        public virtual async Task<TaskResult<IEnumerable<object>>> GetAllAsync()
        {
            var query = await _context.Set<T>().FindAsync(FilterDefinition<T>.Empty);
            query = ApplyFilter(query.AsQueryable()).ToList();

            return ResolveList(query);
        }


        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id">Id of the Entry</param>
        /// <returns>TResponseBase as TaskResult</returns>
        public virtual async Task<TaskResult<object>> GetAsync(string id)
        {
            var resolvedEntity = (await _context.Set<T>().FindAsync(GetFilter(id))).FirstOrDefault();
            if (resolvedEntity == null || resolvedEntity.IsDeleted && !IsAdmin()) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);

            return Resolve(resolvedEntity);
        }

        /// <summary>
        /// Create entity
        /// </summary>
        /// <param name="entity">Entity Data</param>
        /// <returns>TResponseBase as TaskResult</returns>
        public virtual async Task<TaskResult<object>> CreateAsync(TCreate entity)
        {
            var resolvedEntity = _mapper.Map<T>(entity);

            await _context.Set<T>().Collection.InsertOneAsync(resolvedEntity);

            var item = (await _context.Set<T>().FindAsync(GetFilter(resolvedEntity.Id))).FirstOrDefault();
            return Resolve(item!);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="id">Id of the Entry</param>
        /// <param name="entity">Entity data</param>
        /// <returns>TResponseBase as TaskResult</returns>
        public virtual async Task<TaskResult<object>> UpdateAsync(string id, TUpdate entity)
        {
            var resolvedEntity = (await _context.Set<T>().FindAsync(GetFilter(id))).FirstOrDefault();
            if (resolvedEntity == null || resolvedEntity.IsDeleted && !IsAdmin()) return CreateTaskResult.Error<object>(ErrorKey.ENTRY_NOT_FOUND);

            _mapper.Map(entity, resolvedEntity);
            await _context.Set<T>().Collection.ReplaceOneAsync(GetFilter(id), resolvedEntity);

            var item = (await _context.Set<T>().FindAsync(GetFilter(resolvedEntity.Id))).FirstOrDefault();
            return Resolve(item!);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="id">Id of the Entry</param>
        /// <returns>DeleteResponse as TaskResult</returns>
        public virtual async Task<TaskResult<DeleteResponse>> DeleteAsync(string id)
        {
            var resolvedEntity = (await _context.Set<T>().FindAsync(GetFilter(id))).FirstOrDefault();
            if (resolvedEntity == null || resolvedEntity.IsDeleted && !IsAdmin()) return CreateTaskResult.Error<DeleteResponse>(ErrorKey.ENTRY_NOT_FOUND);

            resolvedEntity.IsDeleted = true;
            await _context.Set<T>().Collection.ReplaceOneAsync(GetFilter(id), resolvedEntity);

            return CreateTaskResult.Success(new DeleteResponse
            {
                Id = id.ToString(),
            });
        }
    }
}
