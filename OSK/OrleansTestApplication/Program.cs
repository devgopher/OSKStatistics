using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansTestApplication.Models;

namespace OrleansTestApplication
{
    class Program
    {
        public static int Main(string[] args) => RunMainAsync().Result;

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var clt = new ClientStartup();
                using (var client = await clt.StartClientWithRetries())
                {
                    var rand = new Random(DateTime.Now.Millisecond);

                    var addStatisticsGrain = client.AddStatisticsGrain<Student>();

                    for (int i = 0; i < 10000; ++i)
                    {
                        await addStatisticsGrain.Put(new List<Student>()
                        {
                            new Student()
                            {
                                Name = "Ivan", Surname = "Petrov",
                                Birthdate = DateTime.UtcNow.AddYears(-18 - rand.Next() % 15)
                            }
                        });
                    }

                    Console.WriteLine("Data was added... Getting statistics...");
                    var getStatisticsGrain = client.GetStatisticsGrain<Student>();
                    var stat = await getStatisticsGrain.Get(t => t.Birthdate.Date.Year == 2000);
                    Console.WriteLine($"Data result : {stat.Count} students were born in 2000!");
                }

                Console.ReadKey();
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
