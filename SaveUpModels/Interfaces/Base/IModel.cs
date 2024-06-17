using MongoDB.Bson;

namespace SaveUpModels.Interfaces.Base
{
    public interface IModel : IModelBase
    {
        ObjectId Id { get; set; }
    }
}
