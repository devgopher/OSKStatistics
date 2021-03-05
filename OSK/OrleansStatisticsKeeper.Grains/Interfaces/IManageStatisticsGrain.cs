using Orleans;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IManageStatisticsGrain<T> : IGrainWithGuidKey
        where T : DataChunk
    {
        public Task<bool> Put(T obj);
        public Task<bool> Put(ICollection<T> obj);

        public Task<long> Remove(Func<T, bool> func);
        public Task Remove(T obj);
        public Task<long> Remove(ICollection<T> objs);

        public Task<long> Clean();
    }
}
