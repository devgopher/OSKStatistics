using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineMeteoStatistics.MeteoHttpClient;
using OnlineMeteoStatistics.Settings;

namespace OnlineMeteoStatistics
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var settings = new OnlineMeteoStatisticsSettings();
                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();

                    configuration.GetSection( nameof(OnlineMeteoStatisticsSettings)).Bind(settings);

                    services.AddSingleton(settings);
                    services.AddHttpClient<NarodMonPolling>();
                    services.AddHostedService<NarodMonPolling>();
                });
    }
}
