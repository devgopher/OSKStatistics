using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IProcessStatisticsGrain<T>
        where T : DataChunk
    {
        public Task Process(Func<T, bool> func);
    }
}
