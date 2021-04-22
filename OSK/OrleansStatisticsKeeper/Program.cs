using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AsyncLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.ApplicationParts;
using Orleans.Configuration;
using Orleans.Hosting;
using OrleansStatisticsKeeper.Grains.Grains;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies;
using OrleansStatisticsKeeper.Grains.Utils;
using OrleansStatisticsKeeper.Models;
using OrleansStatisticsKeeper.Models.Settings;
using OrleansStatisticsKeeper.SiloHost.Utils;
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

            if (siloSettings.MaxCpuLoad < 100)
                Task.Run(() => CpuOptimizer.Start(siloSettings.MaxCpuLoad, new CancellationToken()));

            return new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder.UseLocalhostClustering()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton<MongoUtils>();
                            services.AddSingleton(oskSettings);
                            services.AddSingleton(siloSettings);
                            services.AddScoped<IAsyncLogger, NLogLogger>();
                            services.AddSingleton<IAssemblyCache, MemoryAssemblyCache>();
                            services.AddSingleton<IAssemblyMembersCache, MemoryAssemblyMembersCache>();
                        })
                        .Configure((Action<SchedulingOptions>)(options => options.AllowCallChainReentrancy = false))
                        .Configure((Action<ClusterOptions>)(options =>
                        {
                            options.ClusterId = oskSettings.ClusterId;
                            options.ServiceId = oskSettings.ServiceId;
                        }))
                        .Configure((Action<EndpointOptions>)(options => options.AdvertisedIPAddress = IpUtils.IpAddress()))
                        .ConfigureApplicationParts(parts => AddParts(parts, siloSettings).WithReferences())
                        .UseDashboard(options => {
                            options.Host = "*";
                            options.Port = 8080;
                            options.HostSelf = true;
                            options.CounterUpdateIntervalMs = 1000;
                        })
                        .AddMemoryGrainStorage(name: "StatisticsGrainStorage")
                        .AddSimpleMessageStreamProvider("OSKProvider", c => c.OptimizeForImmutableData = true);                    
                })
                .ConfigureLogging(builder => builder.AddConsole())
                .RunConsoleAsync();
        }

        private static Assembly[] GetLinkedAssemblies(SiloSettings siloSettings)
        {
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            var basicDirectory = directoryInfo?.FullName;
            basicDirectory = directoryInfo != null ? directoryInfo.Parent?.Parent?.FullName : basicDirectory;
         
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
            var results = parts
                .AddApplicationPart(typeof(MongoManageStatisticsGrain<>).Assembly)
                .AddApplicationPart(typeof(MongoGetStatisticsGrain<>).Assembly)
                .AddApplicationPart(typeof(DataChunk).Assembly)
                .WithCodeGeneration();

            var linkedAsms = GetLinkedAssemblies(siloSettings);
            foreach (var asm in linkedAsms)
                results.AddApplicationPart(asm);

            return results;
        }
    }
}
