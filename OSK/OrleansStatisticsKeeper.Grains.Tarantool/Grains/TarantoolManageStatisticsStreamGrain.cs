using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.StreamEvents;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Threading.Tasks;
using System.Linq;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models;
using OrleansStatisticsKeeper.Grains.Tarantool.Utils;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Grains
{
    [ImplicitStreamSubscription("OSKNAMESPACE")]
    public class TarantoolManageStatisticsStreamGrain<T> : GenericManageStatisticsStreamGrain<T>, IManageStatisticsStreamGrain<T>
        where T : DataChunk
    {
        private readonly TarantoolUtils _mongoUtils;

        public TarantoolManageStatisticsStreamGrain(TarantoolUtils mongoUtils) => _mongoUtils = mongoUtils;

        public override async Task ProcessCleanRecordsEvent(CleanRecordsEvent<T> @event)
        {
            try
            {
                var collection = await _mongoUtils.GetIndex<T>();
                var delResult = await collection.DeleteManyAsync(d => true);
            }
            catch (Exception)
            {
            }
        }

        public override Task ProcessPutRecordsEvent(PutRecordsEvent<T> @event)
        {
            throw new NotImplementedException();
        }

        public override async Task ProcessRemoveRecordsByConditionEvent(RemoveRecordsByConditionEvent<T> @event)
        {
            try
            {
                var collection = await _mongoUtils.GetIndex<T>();
                var delResult = await collection.DeleteManyAsync(f => @event.ConditionFunc(f));
            }
            catch (Exception)
            {
            }
        }

        public override async Task ProcessRemoveRecordsEvent(RemoveRecordsEvent<T> @event)
        {
            try
            {
                var collection = await _mongoUtils.GetIndex<T>();
                var delResult = await collection.DeleteManyAsync(f => @event.Data.Any(d => f.Id == d.Id));
            }
            catch (Exception)
            {
            }
        }
    }
}
