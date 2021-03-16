using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using OrleansTestApplication.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrleansTestApplication
{
    public class StudentStatistics
    {
        [Benchmark]
        public async Task<int> RunMainAsync()
        {
            try
            {
//#if DEBUG
//                Thread.Sleep(10000);
//#endif
                var clt = new ClientStartup();
                using (var client = await clt.StartClientWithRetries())
                {
                    var rand = new Random(DateTime.Now.Millisecond);

                    // var addStatisticsGrain = client.AddStatisticsGrain<Student>();
                    var addStatisticsGrainPool = new GrainsManageStatisticsPool<Student>(client, 150);
                    var cleanCount = await addStatisticsGrainPool.Clean();
                    
                    Console.WriteLine($"DEL RESULT: {cleanCount}");

                    Console.ReadKey();

                    for (int i = 0; i < 1000000; ++i)
                    {
                        var stud = new Student()
                        {
                            Name = "Ivan",
                            Surname = "Petrov",
                            Birthdate = DateTime.UtcNow.AddYears(-18 - rand.Next() % 15)
                        };

                        await addStatisticsGrainPool.Put(stud);

                       // Console.WriteLine($"student: {JsonConvert.SerializeObject(stud)}");
                    }

                    Console.WriteLine("Data was added... Getting statistics...");
                    //var getStatisticsGrain = client.GetStatisticsGrain<Student>();
                    var getStatisticsGrainPool = new GrainsGetStatisticsPool<Student>(client, 30);
                    var stat = await getStatisticsGrainPool.Get(t => t.Birthdate.Date.Year == 2000);
                    Console.WriteLine($"Data result : {stat.Count} students were born in 2000!");
                    Console.WriteLine($"======================================================");
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return 1;
            }
        }
    }
}
