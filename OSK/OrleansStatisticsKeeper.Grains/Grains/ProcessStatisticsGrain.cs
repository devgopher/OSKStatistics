using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;

namespace OrleansStatisticsKeeper.Grains
{
    public class ProcessStatisticsGrain<T> : Grain, IProcessStatisticsGrain<T>
        where T : DataChunk
    {
        public async Task Process(Func<T, bool> func)
        {

        }
    }
}
