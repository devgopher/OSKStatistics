using System;
using System.Collections.Generic;
using System.Text;
using OrleansStatisticsKeeper.Grains.Models;

namespace ProcessMonitoringService.Models
{
    [Serializable]
    public class MonitoredProcess : DataChunk
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; }
        public string ProcessOwner { get; set; }
        public DateTime DateTime { get; set; }
        public float? CpuLoad { get; set; }

    }
}
