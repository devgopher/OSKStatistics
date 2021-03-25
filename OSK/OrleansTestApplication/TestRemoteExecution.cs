using BenchmarkDotNet.Attributes;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Client.GrainsContext;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using RemoteTestClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
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
            Thread.Sleep(5000);
            var clt = new ClientStartup();
            using (var client = await clt.StartClientWithRetries())
            {
                var rand = new Random(DateTime.Now.Millisecond);

                // var addStatisticsGrain = client.AddStatisticsGrain<Student>();
                var grainsExecutivePool = new GrainsExecutivePool(client, 1);

                await grainsExecutivePool.LoadAssembly(typeof(RemoteExecutionTest));

                var context = new GenericGrainsContext();
                context.SetValue("Add", 1000);

                for (int i = 0; i < 30000; ++i)
                {
                    var ret = await grainsExecutivePool.ExecuteWithContext<decimal>(nameof(RemoteExecutionTest), 
                        nameof(RemoteTestClass.RemoteExecutionTest.PowN), context, (decimal)3, (decimal)i);
                    Console.WriteLine($"RET: {ret}");
                }

                Console.ReadKey();

                return 0;
            }
        }
    }
}
