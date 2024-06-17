using SaveUp.Common.Models;
using SaveUpModels.DTOs.Responses;

namespace SaveUp.Common.Interfaces
{
    public interface IBaseAPIServiceBase
    {
        HttpClient Client { get; }
    }

    public interface IBaseAPIService<TCreateRequest, TUpdateRequest, TResponse> : IBaseAPIServiceBase
        where TCreateRequest : class
        where TUpdateRequest : class
        where TResponse : class
    {
        Task<HTTPResponse<TResponse>> CreateAsync(TCreateRequest data);
        Task<HTTPResponse<DeleteResponse>> DeleteAsync(int id);
        Task<HTTPResponse<List<TResponse>>> GetAllAsync();
        Task<HTTPResponse<TResponse>> GetAsync(int id);
        Task<HTTPResponse<TResponse>> UpdateAsync(int id, TUpdateRequest data);
    }
}