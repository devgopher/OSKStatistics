using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsAddStatisticsPool<T> : IAddStatisticsGrain<T>
    where T : DataChunk
    {
        private readonly IList<IAddStatisticsGrain<T>> _grains;
        private readonly Random rand = new Random(DateTime.Now.Millisecond);

        public GrainsAddStatisticsPool(StatisticsClient client, int poolSize)
        {
            _grains = new List<IAddStatisticsGrain<T>>(poolSize);
            for (int i = 0; i < poolSize; ++i)
                _grains.Add(client.AddStatisticsGrain<T>());
        }


        public async Task<bool> Put(T obj)
            => await _grains[GetGrainNumber()].Put(obj);

        public async Task<bool> Put(ICollection<T> obj)
            => await _grains[GetGrainNumber()].Put(obj);

        private int GetGrainNumber() => rand.Next() % _grains.Count;
    }
}
