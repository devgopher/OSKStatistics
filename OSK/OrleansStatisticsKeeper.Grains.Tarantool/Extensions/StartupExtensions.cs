using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.ApplicationParts;
using OrleansStatisticsKeeper.Grains.MongoBased.Grains;
using OrleansStatisticsKeeper.Grains.Tarantool.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Extensions
{
    public static class StartupExtensions
    {
        public static void AddTarantoolUtils(this IServiceCollection services)
            => services.AddSingleton<TarantoolUtils>();


        public static IApplicationPartManager AddTarantoolGrains(this IApplicationPartManager manager) => 
            manager.AddApplicationPart(typeof(TarantoolManageStatisticsGrain<>).Assembly)
                   .AddApplicationPart(typeof(TarantoolGetStatisticsGrain<>).Assembly);
    }
}
