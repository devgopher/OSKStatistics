using MongoDB.Driver;
using OrleansStatisticsKeeper.Grains.Database;
using OrleansStatisticsKeeper.Grains.Models;
using System.Linq;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Models.Settings;

namespace OrleansStatisticsKeeper.Grains.Utils
{
    public class MongoUtils
    {
        private readonly OskSettings _settings;

        public MongoUtils(OskSettings settings)
        {
            _settings = settings;
        }

        public async Task<IMongoCollection<T>> GetCollection<T>()
            where T : DataChunk
        {
            var typeName = typeof(T).Name;
            var type = typeof(T);
            var mc = ConnectionsFactory.OpenMongo(_settings.ConnectionString);
            var db = mc.GetDatabase(_settings.Database);
            IMongoCollection<T> collection;

            if ((await db.ListCollectionNamesAsync()).ToList().All(c => c != typeName))
            {
                await db.CreateCollectionAsync(typeName);
                collection = db.GetCollection<T>(typeName);

                var indexKeys = Builders<T>.IndexKeys.Ascending(t => t.Id).Descending(t => t.DateTimeTicks);

                //foreach (var prop in type.GetProperties())
                //{
                //    if (Attribute.IsDefined(prop, typeof(IndexedAttribute)))
                //    {
                //        var attrib = (IndexedAttribute)Attribute.GetCustomAttribute(prop, typeof(IndexedAttribute));
                //        var myMethod = prop;
                //        var expr = Expression.Lambda(Expression.Property(Expression.Parameter(prop.PropertyType, prop.Name), prop.Name));
                //        if (attrib.IsAscending)
                //        {
                //            indexKeys.Ascending(expr);
                //        }

                //        indexKeys.
                //    }
                //}

                await collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeys));
            } else
                collection = db.GetCollection<T>(typeName);
            return collection;
        }
    }
}
