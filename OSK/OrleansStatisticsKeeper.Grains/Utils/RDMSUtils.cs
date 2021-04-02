using OrleansStatisticsKeeper.Grains.Database;
using OrleansStatisticsKeeper.Grains.Models;
using OrleansStatisticsKeeper.Models.Settings;
using System.Data;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Utils
{
    public class RDMSUtils
    {
        private readonly OskSettings _settings;
        private readonly string RdmsType;

        public RDMSUtils(OskSettings settings) => _settings = settings;

        public async Task<DataTable> GetData<T>() where T : DataChunk
        {
            using (var rdmsConnection = ConnectionsFactory.OpenRdms(_settings.ConnectionType, _settings.ConnectionString))
            {
                rdmsConnection.Open();
                var collection = rdmsConnection;


                return collection;
            }
        }
    }
}
