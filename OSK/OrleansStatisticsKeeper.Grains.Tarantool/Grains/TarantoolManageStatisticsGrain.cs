using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;
using OrleansStatisticsKeeper.Grains.Tarantool.Utils;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Grains
{
    public class TarantoolManageStatisticsGrain<T> : Grain, IManageStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly TarantoolUtils _tarantoolUtils;

        public TarantoolManageStatisticsGrain(TarantoolUtils mongoUtils) => _tarantoolUtils = mongoUtils;

        public async Task<long> Clean()
        {
            try
            {
                var collection = await _tarantoolUtils.GetIndex<T>();
                var delResult = await collection.DeleteManyAsync(d => true);

                return delResult.DeletedCount;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> Put(T obj)
        {
            try
            {
                await _tarantoolUtils.Upsert(obj);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Put(ICollection<T> objs)
        {
            try
            {
                await _tarantoolUtils.Upsert(objs);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<long> Remove(Func<T, bool> func)
        {
            try
            {
                var collection = await _tarantoolUtils.GetIndex<T>();
                var delResult = await collection.DeleteManyAsync(f => func(f));

                return delResult.DeletedCount;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task Remove(T obj)
        {
            try
            {
                var collection = await _tarantoolUtils.GetIndex<T>();
                var delResult = await collection.DeleteOneAsync(t => t.Id == obj.Id);
            }
            catch
            {
            }
        }

        public async Task Remove(ICollection<T> objs)
        {
            var collection = await _tarantoolUtils.GetIndex<T>();
            var delResult = await collection.DeleteManyAsync(t => objs.Contains(t));
        }
    }
}
