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

        public MongoUtils(OskSettings settings) => _settings = settings;

        public async Task<IMongoCollection<T>> GetCollection<T>()
            where T : DataChunk
        {
            var typeName = typeof(T).Name;
            var mc = ConnectionsFactory.OpenMongo(_settings.ConnectionString);
            var db = mc.GetDatabase(_settings.Database);
            IMongoCollection<T> collection;

            if ((await db.ListCollectionNamesAsync()).ToList().All(c => c != typeName))
            {
                await db.CreateCollectionAsync(typeName);
                collection = db.GetCollection<T>(typeName);

                var indexKeys = Builders<T>.IndexKeys.Ascending(t => t.Id).Descending(t => t.DateTimeTicks);

                await collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeys));
            } else
                collection = db.GetCollection<T>(typeName);
            
            return collection;
        }
    }
}
