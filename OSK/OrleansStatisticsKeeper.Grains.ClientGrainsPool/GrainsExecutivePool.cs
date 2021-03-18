using Ceras;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Diagnostics;
using Utils.Client;
using System.Reflection;
using OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    [Serializable]
    public class GrainsExecutivePool : GrainsPool<IOskGrain>, IOskGrain
    {
        public GrainsExecutivePool(StatisticsClient client, int poolSize) : base(client, poolSize)
        { }

        public async Task<bool> GetIsLoaded(Type type) => _grains.All(g => g.GetIsLoaded(type).Result);

        public async Task<TOUT> Execute<TOUT>(Type type, string funcName, params object[] args)
            => await (await GetGrain()).Execute<TOUT>(type.Name, funcName, args);

        public async Task<TOUT> Execute<TOUT>(string className, string funcName, params object[] args) 
            => await (await GetGrain()).Execute<TOUT>(className, funcName, args);

        /// <summary>
        /// Loads a new assembly!
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public async Task LoadAssembly(Assembly assembly)
        {
            var asmBytes = AssemblyUtils.GetAssemblyBinary(assembly);
            var asmVersion = AssemblyUtils.GetAssemblyVersion(assembly);
            var asmFullname = assembly.FullName;

            await LoadAssembly(asmFullname, asmVersion, asmBytes);
        }
        
        /// <summary>
        /// Loads a new assembly!
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public async Task LoadAssembly(Type targetType)
        {
            var asmVersion = AssemblyUtils.GetAssemblyVersion(targetType);
            var asmFullname = AssemblyUtils.GetAssemblyName(targetType);
            var asmBytes = AssemblyUtils.GetAssemblyBinary(targetType);

            await LoadAssembly(asmFullname, asmVersion, asmBytes);
        }

        /// <summary>
        /// Loads a new assembly
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="version"></param>
        /// <param name="asmPath"></param>
        /// <returns></returns>
        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, string asmPath)
        {
            var tasks = new List<Task>(_grains.Count);
            foreach (var grain in _grains)
                tasks.Add(grain.LoadAssembly(assemblyFullName, version, asmPath));

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Loads a new assembly
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="version"></param>
        /// <param name="asmBytes"></param>
        /// <returns></returns>
        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, byte[] asmBytes)
        {
            // TODO: change it onto direct loading into a cache!
            var tasks = new List<Task>(_grains.Count);
            foreach (var grain in _grains)
                tasks.Add(grain.LoadAssembly(assemblyFullName, version, asmBytes));

            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Resize is blocked temporary, because in this case we need to load all newly 
        /// added grains properly!
        /// </summary>
        /// <param name="poolSize"></param>
        /// <returns></returns>
        public override async Task Resize(int poolSize)
            => throw new NotSupportedException($"{nameof(Resize)} isn't supported for {nameof(GrainsExecutivePool)}!");

    //    protected override async Task<int> GetGrainNumber() => await base.GetGrainNumber();
    }
}