using OrleansStatisticsKeeper.Client.GrainsContext;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteTestClass
{
    public class RemoteExecutionTest : IHasOskRemoteExecutionContext
    {
        public IOskRemoteExecutionContext Context { get; set; }

        public RemoteExecutionTest(IOskRemoteExecutionContext context)
            => Context = context;

        public double PowN(double a, double exp) => Math.Pow(a, exp);
    }
}
