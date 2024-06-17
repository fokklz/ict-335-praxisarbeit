using SaveUpModels.DTOs;
using SaveUpModels.Interfaces.Models;

namespace SaveUpModels.Interfaces
{
    public interface IRefreshRequest<T>
        where T : class, IUserBase
    {
        public T User { get; set; }
        public TokenData TokenData { get; set; }
    }
}
