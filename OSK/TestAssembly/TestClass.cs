using OrleansStatisticsKeeper.Client.GrainsContext;
using System;

namespace TestAssembly
{
    public class TestClass : IHasOskRemoteExecutionContext
    {
        public IOskRemoteExecutionContext Context { get; set; }

        public double Pow2(int a)
            => Math.Pow(a, 2);

        public double Pow3(int a)
            => Math.Pow(a, 3);
    }
}
