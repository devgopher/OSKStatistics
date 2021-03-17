using BenchmarkDotNet.Attributes;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Utils.Client;

namespace OrleansTestApplication
{
    /// <summary>
    /// This class shows us how to organize remote execution of static methods in simple assemblies w/o 
    /// any complex depencies
    /// </summary>
    public class TestRemoteExecution
    {
        [Benchmark]
        public async Task<double> RunMainAsync()
        {
            var clt = new ClientStartup();
            using (var client = await clt.StartClientWithRetries())
            {
                var rand = new Random(DateTime.Now.Millisecond);

                // var addStatisticsGrain = client.AddStatisticsGrain<Student>();
                var grainsExecutivePool = new GrainsExecutivePool(client, 12);

                await grainsExecutivePool.LoadAssembly(typeof(RemoteTestClass.RemoteExecutionTest));
                var ret = await grainsExecutivePool.Execute<double>(nameof(RemoteTestClass.RemoteExecutionTest), 
                    nameof(RemoteTestClass.RemoteExecutionTest.PowN), 3, 4);
                
                Console.ReadKey();

                return ret;
            }
        }
    }
}
