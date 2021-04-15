using OrleansStatisticsKeeper.Client.Services.Settings;

namespace OnlineMeteoStatistics.Commons.Settings
{
    public class OnlineMeteoStatisticsSettings
    {
        public SchedulerSettings SchedulerSettings { get; set; }
        public string NarodMonGuid { get; set; }
        public string NarodApiKey { get; set; }
    }
}
