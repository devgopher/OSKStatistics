using Orleans;
using OrleansStatisticsKeeper.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsPool<T> where T : class, IGrainWithGuidKey
    {
        private readonly IList<T> _grains;
        private readonly Random rand = new Random(DateTime.Now.Millisecond);

        public GrainsPool(StatisticsClient client, int poolSize)
        {
            _grains = new List<T>(poolSize);
            for (int i = 0; i < poolSize; ++i)
                _grains.Add(client.GetGrain<T>());
        }
                private int GetGrainNumber() => rand.Next() % _grains.Count;
        //protected IGrainWithGuidKey GetGrain() => _grains[GetGrainNumber()];

        protected T GetGrain() => _grains[GetGrainNumber()] as T;
    }
}
