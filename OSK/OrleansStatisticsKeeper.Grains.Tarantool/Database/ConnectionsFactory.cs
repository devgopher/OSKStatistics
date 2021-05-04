using ProGaudi.Tarantool.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.MongoBased.Database
{
    public static class ConnectionsFactory
    {
        private static Box _box;
        private static object syncObj = new object();

        public static Box OpenTarantool(string host, int port, string user, string password)
        {
            if (_box == default)
                lock (syncObj)
                {
                    var task = ProGaudi.Tarantool.Client.Box.Connect(host, port, user, password);
                    task.Wait(60000);
                    _box = task.Result;
                }

            return _box;
        }
    }
}
