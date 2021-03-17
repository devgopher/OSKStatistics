using Orleans;
using OrleansStatisticsKeeper.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsPool<T> where T : class, IGrainWithGuidKey
    {
        protected readonly IList<T> _grains;
        private readonly Random rand = new Random(DateTime.Now.Millisecond);

        public GrainsPool(StatisticsClient client, int poolSize)
        {
            _grains = new List<T>(poolSize);
            for (int i = 0; i < poolSize; ++i)
                _grains.Add(client.GetGrain<T>());
        }
        protected virtual async Task<int> GetGrainNumber() => rand.Next() % _grains.Count;
        //protected IGrainWithGuidKey GetGrain() => _grains[GetGrainNumber()];

        protected virtual async Task<T> GetGrain() => _grains[await GetGrainNumber()] as T;
    }
}
