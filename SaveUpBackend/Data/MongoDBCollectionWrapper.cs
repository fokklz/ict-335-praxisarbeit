using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SaveUpModels.Interfaces.Base;
using SaveUpModels.Models;
using System.Reflection;

namespace SaveUpBackend.Data
{
    public class MongoDBCollectionWrapper<T>
        where T : class, IModel
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        private readonly IMapper _mapper;

        private readonly List<BsonDocument> _lookupStages;

        public IMongoCollection<T> Collection { get { return _collection; } }

        public MongoDBCollectionWrapper(IMongoDatabase mongo, IMapper mapper)
        {
            _database = mongo;
            _collection = mongo.GetCollection<T>(ToPlural(typeof(T).Name.ToLower()));

            _mapper = mapper;

            _lookupStages = new List<BsonDocument>();
            foreach (var (_, property, pluralName, foreignKey) in GetPropertiesToProxy())
            {
                // Create the lookup stage
                var lookup = new BsonDocument
                {
                    {
                        "$lookup", new BsonDocument
                        {
                            { "from", pluralName },
                            { "localField", foreignKey },
                            { "foreignField", "_id" },
                            { "as", property.Name.ToLower() }
                        }
                    }
                };
                _lookupStages.Add(lookup);

                // Create the unwind stage
                var unwind = new BsonDocument
                {
                    {
                        "$unwind", new BsonDocument
                        {
                            { "path", $"${property.Name.ToLower()}" },
                            { "preserveNullAndEmptyArrays", true }
                        }
                    }
                };
                _lookupStages.Add(unwind);
            }

        }

        /// <summary>
        /// Define a Index for a field
        /// </summary>
        /// <param name="field">The field to create the index for</param>
        public void SetIndex(string field, bool unique = true)
        {
            if (_collection.Indexes.List().ToList().Any(idx => idx["name"].AsString == $"idx_{field}")) return;
            var index = new CreateIndexModel<T>(Builders<T>.IndexKeys.Ascending(field), new CreateIndexOptions { Unique = unique });
            _collection.Indexes.CreateOne(index);
        }

        /// <summary>
        /// Access another collection based on type
        /// </summary>
        /// <typeparam name="Z">The type to find the collection for (lowercase name in plural will be used as name)</typeparam>
        /// <returns>The Collection for this type</returns>
        public IMongoCollection<Z> GetCollection<Z>()
            where Z : class, IModel
        {
            return _database.GetCollection<Z>(ToPlural(typeof(Z).Name));
        }

        /// <summary>
        /// Seed a single Document into the collection
        /// </summary>
        /// <param name="entities">The entities to create</param>
        /// <returns></returns>
        public async Task<List<ObjectId>> SeedMany(IEnumerable<T> entities)
        {
            var mappedEntities = _mapper.Map<List<T>>(entities);
            await _collection.InsertManyAsync(mappedEntities);
            return mappedEntities.Select(x => x.Id).ToList();
        }

        /// <summary>
        /// Is Any Document inside the collection
        /// </summaryà>
        /// <returns>true if any document is found</returns>
        public async Task<bool> AnyAsync()
        {
            return await CountAsync() > 0;
        }

        public async Task<int> CountAsync()
        {
            return (int)await _collection.CountDocumentsAsync(Builders<T>.Filter.Empty);
        }

        /// <summary>
        /// Find any Document inside the collection
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        /// <returns>a list of found documents</returns>
        public async Task<List<T>> FindAsync(FilterDefinition<T> filter)
        {
            var aggregateFluent = _collection.Aggregate().Match(filter);
            foreach (var lookupStage in _lookupStages)
            {
                aggregateFluent = aggregateFluent.AppendStage<T>(lookupStage);
            }

            return await aggregateFluent.ToListAsync();
        }

        /// <summary>
        /// Find a single document inside the collection
        /// </summary>
        /// <param name="filter">The filter to search for</param>
        /// <returns>The found document based on the filter or null</returns>
        /// <exception cref="InvalidOperationException">Thrown when more than one document is found</exception>
        public async Task<T?> FindSingleAsync(FilterDefinition<T> filter)
        {
            var result = await FindAsync(filter);
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Find a single document by id
        /// </summary>
        /// <param name="id">The id of the target document</param>
        /// <returns>The document or null if not found</returns>
        public async Task<T?> FindByIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var result = await FindAsync(filter);

            return result.FirstOrDefault();
        }

        public async Task<T?> FindByIdAsync(string id)
        {
            return await FindByIdAsync(ObjectId.Parse(id));
        }

        public async Task<ObjectId?> InsertOneAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity.Id;
        }

        public async Task<T> UpdateOneAsync<TUpdate>(ObjectId id, TUpdate update)
            where TUpdate : class
        {
            var filterDef = Builders<T>.Filter.Eq("_id", id);

            var updateDefs = new List<UpdateDefinition<T>>();
            var modelProperties = typeof(T).GetProperties();
            var properties = typeof(TUpdate).GetProperties();
            foreach (var item in properties)
            {
                // find the actual property in the model
                var counterPart = modelProperties.FirstOrDefault(x => x.Name.Equals(item.Name));

                var value = item.GetValue(update);
                if (value != null)
                {
                    // skip the id property for any update
                    var bsonIdAttr = counterPart?.GetCustomAttribute<BsonIdAttribute>();
                    if (bsonIdAttr != null) continue;

                    var bsonElementAttr = counterPart?.GetCustomAttribute<BsonElementAttribute>();
                    var fieldName = bsonElementAttr?.ElementName ?? item.Name;

                    updateDefs.Add(Builders<T>.Update.Set(fieldName, value));
                }
            }
            await _collection.UpdateOneAsync(filterDef, Builders<T>.Update.Combine(updateDefs));
            return (await FindByIdAsync(id))!;


        }

        /// <summary>
        /// Update a document based on the given entity id using the given update model
        /// </summary>
        /// <param name="entity">The entity to Update</param>
        /// <param name="update">The new data in the Update model</param>
        /// <returns>The updated document</returns>
        public async Task<T> UpdateOneAsync<TUpdate>(T entity, TUpdate update)
            where TUpdate : class
        {
            return await UpdateOneAsync(entity.Id, update);
        }

        /// <summary>
        /// Updates a document based on the given entity
        /// </summary>
        /// <param name="entity">THe entity to update</param>
        /// <returns>The updated document</returns>
        public async Task<T> UpdateOneAsync(T entity)
        {
            return await UpdateOneAsync(entity.Id, entity);
        }

        /// <summary>
        /// Resolves all Relations for a entity
        /// </summary>
        /// <param name="entity">The entity to resolve</param>
        /// <returns>The full entity with all data from other collections</returns>
        public async Task<T> ResolveVirtualPropertiesAsync(T entity)
        {
            foreach (var (idProperty, property, pluralName, foreignKey) in GetPropertiesToProxy())
            {
                var virtualPropertyType = property.PropertyType;
                var foreignKeyValue = idProperty.GetValue(entity);

                if (foreignKeyValue != null)
                {
                    var relatedCollection = _database.GetCollection<BsonDocument>(pluralName);
                    var relatedDocument = await relatedCollection.Find(Builders<BsonDocument>.Filter.Eq("_id", foreignKeyValue)).FirstOrDefaultAsync();

                    if (relatedDocument != null)
                    {
                        var relatedEntity = BsonSerializer.Deserialize(relatedDocument, virtualPropertyType);
                        property.SetValue(entity, relatedEntity);
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Helper to get all Properties which should be proxied. The decision is made based on 'virtual' and
        /// the existance of a Id counterpart.
        /// 
        /// Example:
        /// If the model contains a relation which is marked with virtual
        /// and there is a Id property with the same name as prefix
        /// this function will extract all needed information to populate the data
        /// 
        /// public virtual Relation Relation { get; set; }
        /// public ObjectId RelationId { get; set; }
        /// 
        /// </summary>
        /// <returns>a list of all found Properties</returns>
        private List<(PropertyInfo, PropertyInfo, string, string)> GetPropertiesToProxy()
        {
            var proxyTargets = new List<(PropertyInfo, PropertyInfo, string, string)>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties.Where(x =>
                (x.GetGetMethod()?.IsVirtual ?? true) &&
                properties.Any(y => y.Name.Equals($"{x.Name}Id"))))
            {
                var pluralName = ToPlural(property.Name.ToLower());
                var idProperty = properties.First(x => x.Name.Equals($"{property.Name}Id"));

                proxyTargets.Add((idProperty, property, pluralName, $"{property.Name.ToLower()}_id"));
            }

            return proxyTargets;
        }

        /// <summary>
        /// Helper function to get the plural version of a english Word
        /// </summary>
        /// <param name="singular">The singular form</param>
        /// <returns>The converted word</returns>
        private static string ToPlural(string singular)
        {
            // This is a simple implementation and might not cover all cases
            if (string.IsNullOrEmpty(singular))
                return singular;

            if (singular.EndsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                return singular.Substring(0, singular.Length - 1) + "ies";
            }
            else if (singular.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                     singular.EndsWith("sh", StringComparison.OrdinalIgnoreCase) ||
                     singular.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                     singular.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                     singular.EndsWith("z", StringComparison.OrdinalIgnoreCase))
            {
                return singular + "es";
            }
            else
            {
                return singular + "s";
            }
        }
    }
}
