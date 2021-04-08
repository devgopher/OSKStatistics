using System;
using OrleansStatisticsKeeper.Grains.Models;

namespace OnlineMeteoStatistics.Models
{
    [Serializable]
    public class DeviceValues : DataChunk
    {
        public string DeviceId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
        public DateTime DateTimeUtc { get; set; }
    }
}
