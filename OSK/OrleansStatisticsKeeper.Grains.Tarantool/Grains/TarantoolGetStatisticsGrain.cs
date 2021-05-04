using AsyncLogging;
using EntityFrameworkCore.BootKit;
using Newtonsoft.Json;
using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Grains.Tarantool.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Grains
{
    public class TarantoolGetStatisticsGrain<T> : Grain, IGetStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly TarantoolUtils _tarantoolUtils;
        private readonly IAsyncLogger _logger;

        public TarantoolGetStatisticsGrain(TarantoolUtils mongoUtils, IAsyncLogger logger)
        {
            _tarantoolUtils = mongoUtils;
            _logger = logger;
        }

        public async Task<string> GetAllSerialized(GrainCancellationToken cancellationToken = null)
        {
            _logger.Info($"{GetType().Name}.{nameof(GetAllSerialized)}() started...");

            var collection = await _tarantoolUtils.GetIndex<T>();
            if (cancellationToken == null)
                return JsonConvert.SerializeObject(collection);

            var serialized = JsonConvert.SerializeObject(collection);

            return serialized;
        }

        public async Task<T> GetFirst()
        {
            _logger.Info($"{GetType().Name}.{nameof(GetFirst)}() started...");

            var collection = await _tarantoolUtils.GetIndex<T>();
            return collection.FirstOrDefault();
        }

        public async Task<T> GetLast()
        {
            _logger.Info($"{GetType().Name}.{nameof(GetLast)}() started...");

            var collection = await _tarantoolUtils.GetIndex<T>();
            return collection.AsQueryable().TakeLast(1).FirstOrDefault();
        }

        public async Task<bool> Any()
        {
            _logger.Info($"{GetType().Name}.{nameof(Any)}() started...");

            var collection = await _tarantoolUtils.GetIndex<T>();
            return collection.AsQueryable().Any();
        }

        public Task<bool> Any(Func<bool, T> func)
        {
            throw new NotImplementedException();
        }
    }
}
