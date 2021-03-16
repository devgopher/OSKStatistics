using BenchmarkDotNet.Attributes;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrleansTestApplication
{
    public class TestRemoteExecution
    {
        [Benchmark]
        public async Task<int> RunMainAsync()
        {
            Console.WriteLine("Press any key to continue...;");
            Console.ReadKey();
            var clt = new ClientStartup();
            using (var client = await clt.StartClientWithRetries())
            {
                var rand = new Random(DateTime.Now.Millisecond);

                // var addStatisticsGrain = client.AddStatisticsGrain<Student>();
                var grainsExecutivePool = new GrainsExecutivePool(client, 10);
                var ret = await grainsExecutivePool.Execute<int, int>( new Func<int, int>(a => 2*a), 20);

                Console.ReadKey();

                return ret;
            }
        }
    }
}
