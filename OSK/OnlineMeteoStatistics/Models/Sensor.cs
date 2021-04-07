using Newtonsoft.Json;

namespace OnlineMeteoStatistics.Models
{
    public class Sensor
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("mac")]
        public string Mac { get; set; }

        [JsonProperty("fav")]
        public int Fav { get; set; }

        [JsonProperty("pub")]
        public int Pub { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("changed")]
        public int Changed { get; set; }

        [JsonProperty("trend")]
        public int Trend { get; set; }
    }
}
