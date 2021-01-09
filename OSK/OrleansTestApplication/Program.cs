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
                    var statGrain = client.AddStatisticsGrain<Student>();
                    statGrain.Put(new List<Student>()
                    {
                        new Student()
                        {
                            Name = "Ivan", Surname = "Petrov", Birthdate = DateTime.UtcNow.AddYears(-20)
                        }
                    });
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
