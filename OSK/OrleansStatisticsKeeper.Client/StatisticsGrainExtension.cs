using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Client
{
    public static class StatisticsGrainExtension
    {
        public static async Task<ICollection<T>> Get<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk =>
            (await src.GetAll())?.Where(x => func(x)).ToArray();

        public static async Task<T> GetFirst<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk 
            => (await src.GetAll())?.FirstOrDefault(x => func(x));

        public static async Task<ICollection<T>> GetFirstN<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func, int elems)
            where T : DataChunk
            => (await src.GetAll())?.Where(x => func(x)).Take(elems).ToArray();

        public static async Task<T> GetLast<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk
            => (await src.GetAll())?.Where(x => func(x)).Last();

        public static async Task<ICollection<T>> GetLastN<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func, int elems)
            where T : DataChunk
            => (await src.GetAll())?.Where(x => func(x)).TakeLast(elems).ToArray();

        public static async Task<bool> Any<T>(this IGetStatisticsGrain<T> src, Func<T, bool> func)
            where T : DataChunk
            => (await src.GetAll()).Any(x => func(x));
    }
}
