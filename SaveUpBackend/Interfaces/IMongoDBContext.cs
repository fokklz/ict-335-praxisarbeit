using MongoDB.Driver;
using SaveUpBackend.Data;
using SaveUpModels.Interfaces.Base;
using SaveUpModels.Models;

namespace SaveUpBackend.Interfaces
{
    public interface IMongoDBContext
    {
        IMongoClient Client { get; }
        MongoDBCollectionWrapper<Item> Orders { get; }
        MongoDBCollectionWrapper<User> Users { get; }
        MongoDBCollectionWrapper<T> Set<T>() where T : class, IModel;
    }
}