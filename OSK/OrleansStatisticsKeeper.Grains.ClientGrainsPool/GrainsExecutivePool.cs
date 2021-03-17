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

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    [Serializable]
    public class GrainsExecutivePool : GrainsPool<IExecutiveGrain>, IExecutiveGrain
    {
        public GrainsExecutivePool(StatisticsClient client, int poolSize) : base(client, poolSize)
        {
        }

        public async Task<bool> GetIsLoaded() => _grains.Any(g => g.GetIsLoaded().Result);
        public async Task SetIsLoaded(bool val) =>
            throw new NotImplementedException($"{nameof(SetIsLoaded)} won't be implemented for {nameof(GrainsExecutivePool)}!");

        public async Task<TOUT> Execute<TOUT>(string className, string funcName, params object[] args) 
            => await (await GetGrain()).Execute<TOUT>(className, funcName, args);

        public async Task LoadAssembly(string asmPath)
        {
            var tasks = new List<Task>(_grains.Count);
            foreach (var grain in _grains)
                tasks.Add(grain.LoadAssembly(asmPath));

            Task.WaitAll(tasks.ToArray());
        }

        public async Task LoadAssembly(byte[] asmBytes)
        {
            var tasks = new List<Task>(_grains.Count);
            foreach (var grain in _grains)
                tasks.Add(grain.LoadAssembly(asmBytes));

            Task.WaitAll(tasks.ToArray());
        }

        protected override async Task<int> GetGrainNumber()
        {
            int baseNumber = await base.GetGrainNumber();
            while (!(await _grains[baseNumber].GetIsLoaded()))
                baseNumber = await base.GetGrainNumber();

            return baseNumber;
        }
    }
}