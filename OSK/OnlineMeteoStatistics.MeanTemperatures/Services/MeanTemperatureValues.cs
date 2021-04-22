using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OnlineMeteoStatistics.Commons.Models;
using OnlineMeteoStatistics.Commons.Settings;
using OnlineMeteoStatistics.Models;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Client.Services;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;

namespace OnlineMeteoStatistics.MeanTemperatures.Services
{
    public class MeanTemperatureValues : RepeatedTaskHostedService
    {
        private readonly GrainsGetStatisticsPool<DeviceValues> _getStatisticsGrainPool;
        private readonly GrainsManageStatisticsPool<DeviceMeanValues> _addDevicesGrainPool;
        private readonly OnlineMeteoStatisticsSettings _settings;
        private StatisticsClient _client;

        public MeanTemperatureValues(OnlineMeteoStatisticsSettings settings) : base(settings.SchedulerSettings)
        {
            var clt = new ClientStartup();
            _client = clt.StartClientWithRetriesSync();
            _settings = settings;
            _addDevicesGrainPool = new GrainsManageStatisticsPool<DeviceMeanValues>(_client, 10);
            _getStatisticsGrainPool = new GrainsGetStatisticsPool<DeviceValues>(_client, 10);
        }

        public override void InnerTask(string jobName, CancellationToken cancellationToken)
        {
            base.InnerTask(jobName, cancellationToken);
            Console.WriteLine($"{nameof(MeanTemperatureValues)}.{nameof(InnerTask)}() started...");

            var getAllTask = _getStatisticsGrainPool.GetAllCollection();
            getAllTask.Wait(cancellationToken);
            var collection = getAllTask.Result;
            var now = DateTime.UtcNow;
            var values = collection.Where(c => DeltaTime != null && c.DateTimeUtc > now.Add(-DeltaTime.Value) && c.DateTimeUtc <= now).ToArray();
            foreach (var device in values.Select(v => new {Id = v.DeviceId, v.Lat, v.Lon}).Distinct())
            {
                var filtered = values.Where(v => v.DeviceId == device.Id).ToArray();
                if (!filtered.Any())
                    continue;

                var meanValue = filtered.Sum(v => v.Value)/filtered.Length;

                var putTask = _addDevicesGrainPool.Put(new DeviceMeanValues
                {
                    DeviceId = device.Id,
                    Value = meanValue,
                    Lat = device.Lat,
                    Lon = device.Lon
                });

                putTask.Wait(cancellationToken);
            }
        }
    }
}