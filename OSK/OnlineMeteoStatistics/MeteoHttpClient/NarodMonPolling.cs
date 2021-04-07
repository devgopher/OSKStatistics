using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OnlineMeteoStatistics.Models;
using OnlineMeteoStatistics.Settings;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;

namespace OnlineMeteoStatistics.MeteoHttpClient
{
    public class NarodMonPolling : IHostedService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly GrainsManageStatisticsPool<DeviceValues> _addStatisticsGrainPool;
        private readonly OnlineMeteoStatisticsSettings _settings;
        private StatisticsClient _client;
        private const string basicAddresss = "http://narodmon.ru/api/";
        private const string nearBySensorsRequest = "sensorsNearby";

        public NarodMonPolling(OnlineMeteoStatisticsSettings settings, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            var clt = new ClientStartup();
            _client = clt.StartClientWithRetriesSync();
            _settings = settings;
            _addStatisticsGrainPool = new GrainsManageStatisticsPool<DeviceValues>(_client, 10);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var guid = _settings.NarodMonGuid.Replace("-", "");
            var args = $"?lat=55.754500&lon=37.626132&radius=20&types=1&uuid={guid}&api_key={_settings.NarodApiKey}&lang=ru";
            var fullRequest = Url.Combine(basicAddresss, nearBySensorsRequest, args);

            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"{nameof(NarodMonPolling)}.{nameof(StartAsync)}() fullRequest: {fullRequest}");

                using (var clt = _httpClientFactory.CreateClient(nameof(NarodMonPolling)))
                {
                    var res = await clt.GetAsync(fullRequest, cancellationToken);
                    if (res.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"{nameof(NarodMonPolling)}.{nameof(StartAsync)}() status code: OK!");
                        var body = await res.Content.ReadAsStringAsync();
                        var response = JsonConvert.DeserializeObject<SensorsNearbyResponse>(body);

                        //var values = response.Devices.SelectMany(d => d.Sensors,
                        //    (d, s) =>
                        //    {
                        //        if (s == null)
                        //            return new DeviceValues();
                        //        return new DeviceValues()
                        //        {
                        //            DateTimeTicks = d.Time,
                        //            Id = Guid.NewGuid(),
                        //            Lat = d.Lat,
                        //            Lon = d.Lon,
                        //            Unit = s.Unit,
                        //            Value = s.Value
                        //        };
                        //    });

                        var dev = new DeviceValues()
                        {
                            DateTimeTicks = 0,
                            Id = Guid.NewGuid(),
                            Lat = 55.436,
                            Lon = 44.33,
                            Unit = "@",
                            Value = 13.2
                        };

                        await _addStatisticsGrainPool.Put(dev);
                    }
                    else
                        Console.WriteLine($"{nameof(NarodMonPolling)}.{nameof(StartAsync)}() status code: {res.StatusCode}!");
                }

                Thread.Sleep(70000);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
