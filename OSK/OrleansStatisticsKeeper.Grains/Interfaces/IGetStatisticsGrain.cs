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
        public Task<ICollection<T>> Get(Func<T, bool> func);
        public Task<ICollection<T>> GetAll();
        public Task<T> GetFirst(Func<T, bool> func);
        public Task<ICollection<T>> GetFirstN(Func<T, bool> func, int elems);
        public Task<T> GetFirst();
        public Task<T> GetLast(Func<T, bool> func);
        public Task<ICollection<T>> GetLastN(Func<T, bool> func, int elems);
        public Task<T> GetLast();
        public Task<bool> Any(Func<T, bool> func);
        public Task<bool> Any();
    }
}
