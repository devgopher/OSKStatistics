using Orleans;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IExecutiveGrain : IGrainWithGuidKey
    {
        public void LoadAssembly(byte[] asmBytes);
        public Task<TOUT> Execute<TOUT>(string className, string funcName,
            params object[] args);
    }
}
