using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    public class GrainsExecutivePool : GrainsPool<IExecutiveGrain>, IExecutiveGrain
    {
        public GrainsExecutivePool(StatisticsClient client, int poolSize) : base(client, poolSize) 
        { }

        public Task<TOUT> Execute<TIN1, TOUT>(Func<TIN1, TOUT> func, TIN1 in1)
            => GetGrain().Execute(func, in1);

        public Task<TOUT> Execute<TIN1, TIN2, TOUT>(Func<TIN1, TIN2, TOUT> func, TIN1 in1, TIN2 in2)
            => GetGrain().Execute(func, in1, in2);

        public Task<TOUT> Execute<TIN1, TIN2, TIN3, TOUT>(Func<TIN1, TIN2, TIN3, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3)
            => GetGrain().Execute(func, in1, in2, in3);

        public Task<TOUT> Execute<TIN1, TIN2, TIN3, TIN4, TOUT>(Func<TIN1, TIN2, TIN3, TIN4, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3, TIN4 in4)
            => GetGrain().Execute(func, in1, in2, in3, in4);
    }
}