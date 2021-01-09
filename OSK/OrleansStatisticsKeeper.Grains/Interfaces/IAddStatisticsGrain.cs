using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IAddStatisticsGrain<T> : IGrainWithGuidKey
        where T : DataChunk
    {
        public Task<bool> Put(T obj);
        public Task<bool> Put(ICollection<T> obj);
    }
}
