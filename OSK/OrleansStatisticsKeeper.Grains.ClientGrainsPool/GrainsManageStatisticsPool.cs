using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsManageStatisticsPool<T> : IManageStatisticsGrain<T>
    where T : DataChunk
    {
        private readonly IList<IManageStatisticsGrain<T>> _grains;
        private readonly Random rand = new Random(DateTime.Now.Millisecond);

        public GrainsManageStatisticsPool(StatisticsClient client, int poolSize)
        {
            _grains = new List<IManageStatisticsGrain<T>>(poolSize);
            for (int i = 0; i < poolSize; ++i)
                _grains.Add(client.AddStatisticsGrain<T>());
        }

        public async Task<bool> Put(T obj)
            => await _grains[GetGrainNumber()].Put(obj);

        public async Task<bool> Put(ICollection<T> obj)
            => await _grains[GetGrainNumber()].Put(obj);

        public async Task<long> Remove(Func<T, bool> func)
            => await _grains[GetGrainNumber()].Remove(func);

        public async Task Remove(T obj)
            => await _grains[GetGrainNumber()].Remove(obj);

        public async Task<long> Remove(ICollection<T> objs)
             => await _grains[GetGrainNumber()].Remove(objs);

        public async Task<long> Clean()
           => await _grains[GetGrainNumber()].Clean();

        private int GetGrainNumber() => rand.Next() % _grains.Count;
    }
}
