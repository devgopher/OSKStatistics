using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class GenericExecutiveGrain : IExecutiveGrain
    {
        public async Task<TOUT> Execute<TIN1, TOUT>(Func<TIN1, TOUT> func, TIN1 in1)
            => func(in1);

        public async Task<TOUT> Execute<TIN1, TIN2, TOUT>(Func<TIN1, TIN2, TOUT> func, TIN1 in1, TIN2 in2)
            => func(in1, in2);

        public async Task<TOUT> Execute<TIN1, TIN2, TIN3, TOUT>(Func<TIN1, TIN2, TIN3, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3)
            => func(in1, in2, in3);

        public async Task<TOUT> Execute<TIN1, TIN2, TIN3, TIN4, TOUT>(Func<TIN1, TIN2, TIN3, TIN4, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3, TIN4 in4)
            => func(in1, in2, in3, in4);
    }
}
