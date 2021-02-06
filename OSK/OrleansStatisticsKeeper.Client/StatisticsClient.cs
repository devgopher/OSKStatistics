﻿using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using AsyncLogging;

namespace OrleansStatisticsKeeper.Client
{
    public class StatisticsClient : IDisposable
    {
        private readonly IClusterClient _client;
        private readonly IAsyncLogger _logger;

        public StatisticsClient(IClusterClient client, IAsyncLogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public IAddStatisticsGrain<T> AddStatisticsGrain<T>() where T : DataChunk 
            => _client.GetGrain<IAddStatisticsGrain<T>>(Guid.NewGuid());

        public IGetStatisticsGrain<T> GetStatisticsGrain<T>() where T : DataChunk
            => _client.GetGrain<IGetStatisticsGrain<T>>(Guid.NewGuid());

        public void Dispose() => _client?.Dispose();
    }
}
