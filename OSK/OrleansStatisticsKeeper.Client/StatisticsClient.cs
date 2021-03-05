using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;

namespace OrleansStatisticsKeeper.Client
{
    public class StatisticsClient : IDisposable
    {
        private readonly IClusterClient _client;
        public StatisticsClient(IClusterClient client) => _client = client;

        public IManageStatisticsGrain<T> AddStatisticsGrain<T>() where T : DataChunk 
            => _client.GetGrain<IManageStatisticsGrain<T>>(Guid.NewGuid());

        public IGetStatisticsGrain<T> GetStatisticsGrain<T>() where T : DataChunk
            => _client.GetGrain<IGetStatisticsGrain<T>>(Guid.NewGuid());

        public void Dispose() => _client?.Dispose();
    }
}
