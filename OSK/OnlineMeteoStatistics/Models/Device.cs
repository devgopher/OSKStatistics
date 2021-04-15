using System.Collections.Generic;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Grains.Models;

namespace OnlineMeteoStatistics.Models
{
    public class Device : DataChunk
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("my")]
        public int My { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("cmd")]
        public int Cmd { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }

        [JsonProperty("liked")]
        public int Liked { get; set; }

        [JsonProperty("uptime")]
        public int Uptime { get; set; }

        [JsonProperty("sensors")]
        public List<Sensor> Sensors { get; set; }
    }
}