using Microsoft.Extensions.Hosting;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using ProcessMonitoringService.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessMonitoringService
{
    public class ProcessStatistics : BackgroundService, IDisposable
    {
        private GrainsManageStatisticsPool<MonitoredProcess> _addStatisticsGrainPool;
        private StatisticsClient _client;
        private string GetProcessOwner(string processName)
        {
            string query = $"Select * from Win32_Process Where Name = \"{processName}\"";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    string owner = $"{argList[1]}\\{argList[0]}";
                    return owner;
                }
            }

            return "NO OWNER";
        }

        public ProcessStatistics()
        {
            var clt = new ClientStartup();
            _client = clt.StartClientWithRetriesSync();
            _addStatisticsGrainPool = new GrainsManageStatisticsPool<MonitoredProcess>(_client, 10);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var runningProcesses = Process.GetProcesses();

            var mps = runningProcesses.Select(p => new MonitoredProcess()
            {
                ProcessId = p.Id,
                ProcessName = p.ProcessName,
                DateTime = DateTime.Now,
                ProcessOwner = GetProcessOwner(p.ProcessName),
                CpuLoad = Utils.Process.ProcessCpuCounter.GetPerfCounterForProcessId(p.Id)?.NextValue()
            });

            foreach (var prc in mps)
                await _addStatisticsGrainPool.Put(prc);

            Thread.Sleep(60000);
        }

        public override void Dispose()
        {
            base.Dispose();
            _client.Dispose();
        }
    }
}