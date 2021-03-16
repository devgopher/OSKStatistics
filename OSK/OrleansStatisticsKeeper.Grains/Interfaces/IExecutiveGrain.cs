using Orleans;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Interfaces
{
    public interface IExecutiveGrain : IGrainWithGuidKey
    {
        public Task<TOUT> Execute<TIN1, TOUT>(Func<TIN1, TOUT> func, TIN1 in1);
        public Task<TOUT> Execute<TIN1, TIN2, TOUT>(Func<TIN1, TIN2, TOUT> func, TIN1 in1, TIN2 in2);
        public Task<TOUT> Execute<TIN1, TIN2, TIN3, TOUT>(Func<TIN1, TIN2,TIN3, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3);
        public Task<TOUT> Execute<TIN1, TIN2, TIN3, TIN4, TOUT>(Func<TIN1, TIN2, TIN3, TIN4, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3, TIN4 tin4);
    }
}
