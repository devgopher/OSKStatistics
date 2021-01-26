using Orleans;
using OrleansStatisticsKeeper.Grains.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
