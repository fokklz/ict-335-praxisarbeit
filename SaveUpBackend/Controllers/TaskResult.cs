using SaveUpBackend.Common.Enums;
using SaveUpModels.DTOs;


#nullable disable

namespace SaveUpBackend.Common
{
    public class TaskResultBase
    {
        public ErrorData Error { get; set; }
        public Exception Exception { get; set; }
        public bool IsSuccess { get; set; }
    }

    /// <summary>
    /// Reutrn Wrapper for all Task Results inside services to allow for better error handling
    /// and simpler implementation in controllers
    /// </summary>
    /// <typeparam name="T">The actual Response</typeparam>
    public class TaskResult<T> : TaskResultBase
        where T : class
    {
        public T Result { get; private set; }

        /// <summary>
        /// Truthy state constructor for TaskResult
        /// </summary>
        /// <param name="response">The response to pass</param>
        public TaskResult(T response)
        {
            IsSuccess = true;
            Result = response;
            Error = new ErrorData()
            {
                Code = "UNKNOWN",
                Message = "UNKNOWN"
            };
        }

        /// <summary>
        /// Falsy state constructor for TaskResult
        /// </summary>
        /// <param name="key">The <see cref="ErrorKey"/> to use</param>
        public TaskResult(ErrorKey key, Exception exception)
        {
            IsSuccess = false;
            Result = default;
            Error = new ErrorData
            {
                Code = key.ToString(),
                Message = key.ToString()
            };
            Exception = exception;
        }

        /// <summary>
        /// Falsy state constructor for TaskResult from another TaskResult
        /// </summary>
        /// <param name="old">The old TaskResult to use</param>
        /// <param name="errorKey">The <see cref="ErrorKey"/> to use</param>
        public TaskResult(TaskResultBase old, ErrorKey? errorKey = null)
        {
            if (!old.IsSuccess)
            {
                Error = errorKey == null ? old.Error : new ErrorData
                {
                    Code = errorKey.ToString(),
                    Message = errorKey.ToString()
                };
                Exception = old.Exception;
                IsSuccess = false;
                Result = default;
            }
        }

    }

    /// <summary>
    /// Helper class to create TaskResults
    /// </summary>
    public static class CreateTaskResult
    {
        /// <summary>
        /// Successfull TaskResult
        /// </summary>
        /// <typeparam name="T">The type of TaskResult to use</typeparam>
        /// <param name="response">The response to pass</param>
        /// <returns>a Truthy instance of <see cref="TaskResult{T}"/></returns>
        public static TaskResult<T> Success<T>(T response)
            where T : class
        {
            return new TaskResult<T>(response);
        }

        /// <summary>
        /// Error TaskResult
        /// </summary>
        /// <typeparam name="T">The type of TaskResult to use</typeparam>
        /// <param name="key">the <see cref="ErrorKey"/> to use</param>
        /// <returns>a Falsy instance of <see cref="TaskResult{T}"/></returns>
        public static TaskResult<T> Error<T>(ErrorKey key, Exception e = null)
            where T : class
        {
            return new TaskResult<T>(key, e);
        }

        /// <summary>
        /// Error TaskResult from another TaskResult
        /// </summary>
        /// <typeparam name="T">The type of TaskResult to use</typeparam>
        /// <param name="old">The old TaskResult to create the now one From</param>
        /// <param name="errorKey">The <see cref="ErrorKey"/> to use</param>
        /// <returns>a Falsy instance of <see cref="TaskResult{T}"/></returns>
        public static TaskResult<T> Error<T>(TaskResultBase old, ErrorKey? errorKey = null)
            where T : class
        {
            return new TaskResult<T>(old, errorKey);
        }
    }


}
