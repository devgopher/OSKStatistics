using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Grains.MongoBased.Database;
using OrleansStatisticsKeeper.Models.Settings;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;
using ProGaudi.Tarantool.Client.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Tarantool.Utils
{
    public class TarantoolUtils
    {
        private readonly OskSettings _settings;

        public TarantoolUtils(OskSettings settings) => _settings = settings;


        public async Task<ICollection<T>> GetCollection<T>()
            where T : DataChunk
        {
            var ttConnection = GetTtConnection();
            var index = GetSpace<T>(ttConnection)["osk_statistics"];
            var all = await index.Select<Guid, T>(Guid.Empty, new SelectOptions() { Iterator = Iterator.All });

            return all?.Data;
        }

        public async Task Upsert<T>(T obj)
            where T : DataChunk
        {
            var ttConnection = GetTtConnection();
            var index = GetSpace<T>(ttConnection)["osk_statistics"];
            await index.Insert<T>(obj);
        }

        public async Task Upsert<T>(ICollection<T> objs)
            where T : DataChunk
        {
            IIndex index = InnerGetIndex<T>();
            await index.Insert(objs);
        }

        public async Task Remove<T>(ICollection<T> objs)
            where T : DataChunk
        {
            IIndex index = InnerGetIndex<T>();

            foreach (var obj in objs)
                await index.Delete<Guid, T>(obj.Id);
        }

        public async Task Remove<T>(T obj)
            where T : DataChunk
        {
            IIndex index = InnerGetIndex<T>();
            await index.Delete<Guid, T>(obj.Id);
        }

        public async Task Remove<T>(Func<T, bool> func)
            where T : DataChunk
        {
            IIndex index = InnerGetIndex<T>();
            var objs = await index.Select<Guid, T>(Guid.Empty, new SelectOptions() { Iterator = Iterator.All });

            foreach (var obj in objs?.Data.Where(o => func(o)))
                await index.Delete<Guid, T>(obj.Id);
        }

        private ProGaudi.Tarantool.Client.Box GetTtConnection() 
            => ConnectionsFactory.OpenTarantool(_settings.ConnectionString);

        public async Task<ICollection<T>> GetIndex<T>()
            where T : DataChunk
        {
            var ttConnection = GetTtConnection();
            var index = GetSpace<T>(ttConnection)["osk_statistics"];
            var all = await index.Select<Guid, T>(Guid.Empty, new SelectOptions() { Iterator = Iterator.All });

            return all?.Data;
        }

        private static ISpace GetSpace<T>(Box ttConnection) where T : DataChunk
            => ttConnection.GetSchema()[typeof(T).Name];

        private IIndex InnerGetIndex<T>() 
            where T : DataChunk
        {
            var ttConnection = GetTtConnection();
            var index = GetSpace<T>(ttConnection)["osk_statistics"];
            return index;
        }
    }
}
