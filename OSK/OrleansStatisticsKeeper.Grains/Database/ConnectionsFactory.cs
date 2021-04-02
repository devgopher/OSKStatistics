using MongoDB.Driver;

namespace OrleansStatisticsKeeper.Grains.Database
{
    public static class ConnectionsFactory
    {
        private static MongoClient _mongoClient;
        public static MongoClient OpenMongo(string connString)
        {
            if (_mongoClient == null)
                _mongoClient = new MongoClient(connString);
            return _mongoClient;
        }
    }
}
