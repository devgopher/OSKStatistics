using System.Collections.Generic;
using Newtonsoft.Json;

namespace OnlineMeteoStatistics.Models
{
    public class SensorsNearbyResponse
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; set; }
    }
}