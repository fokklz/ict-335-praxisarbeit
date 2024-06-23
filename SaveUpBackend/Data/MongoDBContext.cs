using AutoMapper;
using MongoDB.Driver;
using SaveUpBackend.Interfaces;
using SaveUpModels.Common.Helpers;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.Enums;
using SaveUpModels.Interfaces.Base;
using SaveUpModels.Models;
using System.Diagnostics;

namespace SaveUpBackend.Data
{
    public class MongoDBContext : IMongoDBContext
    {

        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IServiceProvider _serviceProvider;

        private readonly IMapper _mapper;

        public IMongoClient Client { get { return _client; } }

        public MongoDBContext(IConfiguration configuration, IMapper mapper, IServiceProvider serviceProvider)
        {
            MongoDBInitialization.RegisterMappings();
            var mongoConfig = configuration.GetSection("Databases").GetSection("MongoDB");

            _client = new MongoClient(mongoConfig.GetValue("URL", "mongodb://localhost:27017/SaveUp")!);
            _database = _client.GetDatabase(mongoConfig.GetValue("Name", "SaveUp"));

            _mapper = mapper;

            Users.SetIndex("Username");
        }

        public MongoDBCollectionWrapper<User> Users => new(_database, _mapper);
        public MongoDBCollectionWrapper<Item> Orders => new(_database, _mapper);


        /// <summary>
        /// Dynamically load a Collection wrapper based on a Type
        /// </summary>
        /// <typeparam name="T">Target Type to get the Wrapper for</typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown when the type has no Wrapper declaration inside this class</exception>
        public MongoDBCollectionWrapper<T> Set<T>()
            where T : class, IModel
        {
            var propertyInfo = GetType()
                                .GetProperties()
                                .FirstOrDefault(p => p.PropertyType == typeof(MongoDBCollectionWrapper<T>));

            if (propertyInfo != null)
            {
                return (MongoDBCollectionWrapper<T>)propertyInfo.GetValue(this)!;
            }
            else
            {
                // will only every throw if a undeclared collection is getting received
                throw new InvalidOperationException($"No collection wrapper found for type {typeof(T).Name}");
            }
        }

    }
}
