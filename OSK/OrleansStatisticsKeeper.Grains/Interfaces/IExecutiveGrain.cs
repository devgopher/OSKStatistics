using Orleans;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IExecutiveGrain : IGrainWithGuidKey
    {
        public Task<bool> GetIsLoaded(Type type);
        public Task LoadAssembly(string assemblyFullName, FileVersionInfo version, string asmPath);
        public Task LoadAssembly(string assemblyFullName, FileVersionInfo version, byte[] asmBytes);
        public Task<TOUT> Execute<TOUT>(string className, string funcName, params object[] args);
    }
}
