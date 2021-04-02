using Microsoft.Data.Sqlite;
using MongoDB.Driver;
using System.Data;

namespace OrleansStatisticsKeeper.Grains.Database
{
    public static class ConnectionsFactory
    {
        public static MongoClient OpenMongo(string connString) => new MongoClient(connString);
        public static IDbConnection OpenRdms(string type, string connString)
            => type.ToLower() switch
            {
                "sqlite" => new SqliteConnection(connString),
                _ => null,
            };
    }
}
