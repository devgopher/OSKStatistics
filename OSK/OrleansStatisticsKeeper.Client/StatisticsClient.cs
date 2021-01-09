using System;
using System.Collections.Generic;
using System.Text;
using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models.Settings;

namespace OrleansStatisticsKeeper.Client
{
    public class StatisticsClient : IDisposable
    {
        private readonly IClusterClient _client;
        private readonly OskSettings _settings;
        public StatisticsClient(IClusterClient client, OskSettings settings)
        {
            _client = client;
            _settings = settings;
        }

        public IAddStatisticsGrain<T> AddStatisticsGrain<T>() where T : DataChunk 
            => _client.GetGrain<IAddStatisticsGrain<T>>(Guid.NewGuid());

        public IGetStatisticsGrain<T> GetStatisticsGrain<T>() where T : DataChunk
            => _client.GetGrain<IGetStatisticsGrain<T>>(Guid.NewGuid());

        public void Dispose() => _client?.Dispose();
    }
}
