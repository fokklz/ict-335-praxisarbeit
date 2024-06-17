using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using SaveUpModels.Interfaces;
using SaveUpModels.Models;

namespace SaveUpModels.Common.Helpers
{
    public class MongoDBInitialization
    {
        private static bool _isInitialized = false;

        public static void RegisterMappings()
        {
            if (_isInitialized) return;

            RegisterMapping<IItem, Item>();
            RegisterMapping<IUser, User>();

            _isInitialized = true;
        }

        private static void RegisterMapping<TInterface, TConcrete>(bool root = false) where TConcrete : class, TInterface
        {
            BsonClassMap.RegisterClassMap<TConcrete>(cm =>
            {
                cm.AutoMap();
                if (root) cm.SetIsRootClass(true);
            });

            BsonSerializer.RegisterSerializer(typeof(TInterface), new ImpliedImplementationInterfaceSerializer<TInterface, TConcrete>());
        }
    }
}
