using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public interface IAssemblyCache
    {
        public bool Exists(string fullName);
        public Assembly Get(string fullName);
        public void Set(Assembly assembly);
    }
}
