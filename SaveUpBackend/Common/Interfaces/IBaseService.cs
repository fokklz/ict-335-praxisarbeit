using SaveUpModels.DTOs.Responses;
using SaveUpModels.Interfaces.Base;

namespace SaveUpBackend.Common.Interfaces
{
    public interface IBaseService<T, TResponse, TUpdate, TCreate>
        where T : class, IModel
        where TResponse : class
        where TUpdate : class
        where TCreate : class
    {
        bool IsOwnerOrAdmin<TModel>(TModel? item, bool allowAdmin = true)
            where TModel : class, IModel;
        Task<bool> IsOwnerOrAdmin(string id, bool allowAdmin = true);
        Task<TaskResult<IEnumerable<object>>> GetAllAsync();
        Task<TaskResult<object>> GetAsync(string id);
        Task<TaskResult<object>> CreateAsync(TCreate entity);
        Task<TaskResult<object>> UpdateAsync(string id, TUpdate entity);
        Task<TaskResult<DeleteResponse>> DeleteAsync(string id);
    }
}
