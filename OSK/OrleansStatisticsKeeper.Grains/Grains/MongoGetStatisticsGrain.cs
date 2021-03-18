using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncLogging;
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
        private readonly IAsyncLogger _logger;

        public MongoGetStatisticsGrain(MongoUtils mongoUtils, IAsyncLogger logger)
        {
            _mongoUtils = mongoUtils;
            _logger = logger;
        }

        public async Task<ICollection<T>> GetAll(GrainCancellationToken cancellationToken = null)
        {
            _logger.Info($"{this.GetType().Name}.{nameof(GetAll)}() started...");

            var collection = await _mongoUtils.GetCollection<T>();
            return await collection.AsQueryable().ToListAsync(cancellationToken.CancellationToken);
        }

        public async Task<T> GetFirst()
        {
            _logger.Info($"{this.GetType().Name}.{nameof(GetFirst)}() started...");

            var collection = await _mongoUtils.GetCollection<T>();
            return collection.FirstOrDefault();
        }

        public async Task<T> GetLast()
        {
            _logger.Info($"{this.GetType().Name}.{nameof(GetLast)}() started...");

            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().TakeLast(1).FirstOrDefault();
        }

        public async Task<bool> Any()
        {
            _logger.Info($"{this.GetType().Name}.{nameof(Any)}() started...");

            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().Any();
        }
    }
}
