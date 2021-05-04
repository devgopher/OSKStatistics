using Microsoft.Data.Sqlite;
using MongoDB.Driver;
using OrleansStatisticsKeeper.Grains.Database;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Database
{
    public static class ConnectionsFactory
    {
        private static MongoClient _mongoClient;
        private static IDictionary<string, IDbConnection> _dbConnections = new Dictionary<string, IDbConnection>(20);
        private static object syncObj = new object();

        public static MongoClient OpenMongo(string connString)
        {
            if (_mongoClient == default)
                lock (syncObj)
                    _mongoClient = new MongoClient(connString);

            return _mongoClient;
        }
    }
}
