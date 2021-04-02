using OrleansStatisticsKeeper.Client.GrainsContext;
using System;
using System.Collections.Generic;
using System.Text;
using DecimalMath;

namespace RemoteTestClass
{
    public class RemoteExecutionTest : IHasOskRemoteExecutionContext
    {
        public IOskRemoteExecutionContext Context { get; set; }

        public RemoteExecutionTest(IOskRemoteExecutionContext context)
            => Context = context;

        public decimal PowN(decimal a, decimal exp) => DecimalEx.Pow(a, exp) + Context.GetValue<decimal>("Add");
    }
}
