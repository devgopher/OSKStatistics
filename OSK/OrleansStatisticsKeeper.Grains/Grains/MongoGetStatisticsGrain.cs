using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.BootKit;
using MongoDB.Driver;
using OrleansStatisticsKeeper.Grains.Models;
using MongoUtils = OrleansStatisticsKeeper.Grains.Utils.MongoUtils;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class MongoGetStatisticsGrain<T> : IGetStatisticsGrain<T>
        where T : DataChunk
    {
        private readonly MongoUtils _mongoUtils;

        public MongoGetStatisticsGrain(MongoUtils mongoUtils) => _mongoUtils = mongoUtils;

        public async Task<ICollection<T>> Get(Func<T, bool> func)
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return await collection.Where(x => func(x)).ToListAsync();
        }

        public async Task<ICollection<T>> GetAll()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return await collection.Where(x => true).ToListAsync();
        }

        public async Task<T> GetFirst(Func<T, bool> func)
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.FirstOrDefault(x => func(x));
        }
        
        public async Task<ICollection<T>> GetFirstN(Func<T, bool> func, int elems)
            => (await Get(func)).Take(elems).ToList();

        public async Task<T> GetFirst()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.FirstOrDefault();
        }

        public async Task<T> GetLast(Func<T, bool> func)
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.Where(x => func(x)).TakeLast(1).FirstOrDefault();
        }

        public async Task<ICollection<T>> GetLastN(Func<T, bool> func, int elems)
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.Where(x => func(x)).TakeLast(elems).ToList();
        }

        public async Task<T> GetLast()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().TakeLast(1).FirstOrDefault();
        }

        public async Task<bool> Any(Func<T, bool> func)
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().Any(func);
        }

        public async Task<bool> Any()
        {
            var collection = await _mongoUtils.GetCollection<T>();
            return collection.AsQueryable().Any();
        }
    }
}
