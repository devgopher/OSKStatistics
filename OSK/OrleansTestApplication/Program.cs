using OrleansStatisticsKeeper.Client;
using OrleansTestApplication.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrleansStatisticsKeeper.Grains.ClientGrainsPool;
using Newtonsoft.Json;

namespace OrleansTestApplication
{
    class Program
    {
        public static int Main(string[] args) =>  (int)(new TestRemoteExecution()).RunMainAsync().Result;

    }
}
