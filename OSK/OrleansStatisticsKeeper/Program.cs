﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansStatisticsKeeper.Grains.Grains;
using OrleansStatisticsKeeper.Grains.Utils;
using OrleansStatisticsKeeper.Models.Settings;
using Utils;

namespace OrleansStatisticsKeeper
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var oskSettings = new OskSettings();
            var siloSettings = new SiloSettings();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            configuration.GetSection("OskSettings").Bind(oskSettings);
            configuration.GetSection("SiloSettings").Bind(siloSettings);

            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder.UseLocalhostClustering()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton<MongoUtils>();
                            services.AddSingleton(oskSettings);
                            services.AddSingleton(siloSettings);
                        })
                        .Configure((System.Action<SchedulingOptions>)(options => options.AllowCallChainReentrancy = false))
                        .Configure((System.Action<ClusterOptions>)(options =>
                        {
                            options.ClusterId = oskSettings.ClusterId;
                            options.ServiceId = oskSettings.ServiceId;
                        }))
                        .Configure((System.Action<EndpointOptions>)(options => options.AdvertisedIPAddress = IpUtils.IpAddress()))
                        .ConfigureApplicationParts(parts => AddParts(parts, siloSettings)
                            .WithReferences())
                        //.AddPerfCountersTelemetryConsumer()
                        .AddMemoryGrainStorage(name: "StatisticsGrainStorage");
                    //.AddGrainService<DataGrainService>()
                })
                .ConfigureLogging(builder => builder.AddConsole())
                .RunConsoleAsync();
        }

        private static Assembly[] GetLinkedAssemblies(SiloSettings siloSettings)
        {
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            var basicDirectory = directoryInfo?.FullName;
            basicDirectory = directoryInfo != null ? directoryInfo?.Parent?.Parent?.FullName : basicDirectory;
         
            var asmPaths = siloSettings.ModelsAssemblies?.SelectMany(x => Directory.GetFiles(basicDirectory, x, SearchOption.AllDirectories));

            var asms = new List<Assembly>();
            if (asmPaths == null) 
                return asms.ToArray();
            
            foreach (var asmPath in asmPaths)
            {
                try
                {
                    var asm = Assembly.LoadFrom(asmPath);
                    asms.Add(asm);
                    Console.WriteLine($"Loaded assembly: {asmPath}!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Can't load assembly: {asmPath}, {ex.Message}!");
                }
            }

            return asms.ToArray();
        }

        private static IApplicationPartManagerWithAssemblies AddParts(IApplicationPartManager parts, SiloSettings siloSettings)
        {
            var results= parts
                      .AddApplicationPart(typeof(MongoAddStatisticsGrain<>).Assembly)
                      .AddApplicationPart(typeof(MongoGetStatisticsGrain<>).Assembly);

            var linkedAsms = GetLinkedAssemblies(siloSettings);
            foreach (var asm in linkedAsms)
            {
                results.AddApplicationPart(asm);
            }

            return results;
        }
    }
}
