using MongoDB.Driver;

namespace OrleansStatisticsKeeper.Grains.Database
{
    public static class ConnectionsFactory
    {
        public static MongoClient OpenMongo(string connString) => new MongoClient(connString);
    }
}
