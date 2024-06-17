using MongoDB.Bson;
using SaveUpModels.Interfaces;
using SaveUpModels.Interfaces.Base;

namespace SaveUpModels.DTOs.Requests.Base
{
    public class CreateRequest : IModel, ICreateDTO
    {
        // Hidden properties since they are not allowed to be set when creating
        bool IModelBase.IsDeleted { get; set; }
        ObjectId IModel.Id { get; set; }
    }
}
