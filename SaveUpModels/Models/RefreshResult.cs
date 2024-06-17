using SaveUpModels.DTOs;
using SaveUpModels.Interfaces;

namespace SaveUpModels.Models
{
    public class RefreshResult : IRefreshRequest<User>
    {
        public required User User { get; set; }
        public required TokenData TokenData { get; set; }
    }
}
