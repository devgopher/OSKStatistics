using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.ApplicationParts;
using OrleansStatisticsKeeper.Grains.MongoBased.Grains;
using OrleansStatisticsKeeper.Grains.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Extensions
{
    public static class StartupExtensions
    {
        public static void AddMongoUtils(this IServiceCollection services)
            => services.AddSingleton<MongoUtils>();


        public static IApplicationPartManager AddMongoGrains(this IApplicationPartManager manager) => 
            manager.AddApplicationPart(typeof(MongoManageStatisticsGrain<>).Assembly)
                   .AddApplicationPart(typeof(MongoGetStatisticsGrain<>).Assembly);
    }
}
