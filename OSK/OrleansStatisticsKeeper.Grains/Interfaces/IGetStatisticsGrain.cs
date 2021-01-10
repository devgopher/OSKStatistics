using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IGetStatisticsGrain<T> : IGrainWithGuidKey
        where T : DataChunk
    {
        public Task<ICollection<T>> GetAll();
        public Task<T> GetFirst();
        public Task<T> GetLast();
        public Task<bool> Any();
    }
}
