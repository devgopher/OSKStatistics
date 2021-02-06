using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AsyncLogging;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Grains.Models;


namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class MongoAddStatisticsGrain<T> : Grain, IAddStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly MongoUtils _mongoUtils;
        private readonly IAsyncLogger _logger;

        public MongoAddStatisticsGrain(MongoUtils mongoUtils, IAsyncLogger logger)
        {
            _mongoUtils = mongoUtils;
            _logger = logger;
        }

        public async Task<bool> Put(T obj)
        {
            try
            {
                _logger.Info($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) started...");
                var collection = await _mongoUtils.GetCollection<T>();
                _logger.Info($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) got collection <{nameof(T)}>");
                await collection.InsertOneAsync(obj);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) insert FAILED", ex);
                return false;
            }

            _logger.Info($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) insert OK");
            return true;
        }

        public async Task<bool> Put(ICollection<T> obj)
        {
            try
            {
                _logger.Info($"{this.GetType().Name}.{nameof(Put)}({nameof(T)})) started...");
                var collection = await _mongoUtils.GetCollection<T>();
                _logger.Info($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) got collection <{nameof(T)}>");
                await collection.InsertManyAsync(obj);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) insert FAILED", ex);
                return false;
            }

            _logger.Info($"{this.GetType().Name}.{nameof(Put)}({JsonConvert.SerializeObject(obj)}) insert OK");
            return true;
        }
    }
}
