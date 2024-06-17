using SaveUpModels.DTOs;
using System.Net;

namespace SaveUp.Common.Interfaces
{
    public interface IHTTPResponse<TResponse>
    {
        bool IsSuccess { get; }
        HttpStatusCode StatusCode { get; }

        Task<ErrorData?> ParseError();
        Task<TResponse?> ParseSuccess();
    }
}