using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsGetStatisticsPool<T> : IGetStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly IList<IGetStatisticsGrain<T>> _grains;
        private readonly Random rand = new Random(DateTime.Now.Millisecond);

        public GrainsGetStatisticsPool(StatisticsClient client, int poolSize)
        {
            _grains = new List<IGetStatisticsGrain<T>>(poolSize);
            for (int i = 0; i < poolSize; ++i)
                _grains.Add(client.GetStatisticsGrain<T>());
        }

        private int GetGrainNumber() => rand.Next() % _grains.Count;

        public async Task<ICollection<T>> GetAll()
            => await _grains[GetGrainNumber()].GetAll();

        public async Task<T> GetFirst()
            => await _grains[GetGrainNumber()].GetFirst();

        public async Task<T> GetLast()
            => await _grains[GetGrainNumber()].GetLast();

        public async Task<bool> Any()
            => await _grains[GetGrainNumber()].Any();
    }
}