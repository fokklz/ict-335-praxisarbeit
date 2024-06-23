using MongoDB.Bson;
using SaveUpModels.Interfaces.Base;
using SaveUpModels.Interfaces.Models;

namespace SaveUpModels.Interfaces
{
    public interface IItem : IModel, IItemBase
    {
        ObjectId UserId { get; set; }
    }
}
