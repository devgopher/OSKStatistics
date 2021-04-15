using OrleansStatisticsKeeper.Grains.Models;
using System;

namespace OnlineMeteoStatistics.Models
{
    [Serializable]
    public class DeviceMeanValues : DataChunk
    {
        public string DeviceId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public int PeriodInSec { get; set; }
    }
}
