using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.BootKit;
using MongoDB.Driver;
using Orleans;
using OrleansStatisticsKeeper.Grains.Models;
using MongoUtils = OrleansStatisticsKeeper.Grains.Utils.MongoUtils;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class MongoGetStatisticsGrain<T> : Grain, IGetStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly MongoUtils _mongoUtils;

        public MongoGetStatisticsGrain(MongoUtils mongoUtils) => _mongoUtils = mongoUtils;

        public async Task<ICollection<T>> GetAll()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return await collection.Where(x => true).ToListAsync();
        }

        public async Task<T> GetFirst()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.FirstOrDefault();
        }

        public async Task<T> GetLast()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().TakeLast(1).FirstOrDefault();
        }

        public async Task<bool> Any()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().Any();
        }
    }
}
