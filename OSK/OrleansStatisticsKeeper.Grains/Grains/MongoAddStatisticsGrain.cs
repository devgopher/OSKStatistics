using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;


namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class MongoAddStatisticsGrain<T> : Grain, IAddStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly MongoUtils _mongoUtils;

        public MongoAddStatisticsGrain(MongoUtils mongoUtils) => _mongoUtils = mongoUtils;

        public async Task<bool> Put(T obj)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                await collection.InsertOneAsync(obj);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Put(ICollection<T> obj)
        {
            try
            {
                var collection = await _mongoUtils.GetCollection<T>();
                await collection.InsertManyAsync(obj);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
