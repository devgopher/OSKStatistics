using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils.Client;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public class MemoryAssemblyCache : IAssemblyCache
    {
        private readonly ConcurrentDictionary<string, Assembly> _assemblies
            = new ConcurrentDictionary<string, Assembly>();

        public bool Exists(string fullName)
            => _assemblies.Any(a => (a.Key == fullName));

        public Assembly Get(string fullName)
        {
            if (!Exists(fullName))
                return default;
            var kvp = _assemblies.FirstOrDefault(a => (a.Key == fullName));
            return kvp.Value;
        }

        public void Set(Assembly assembly)
        {
            var fullName = assembly.FullName;
            _assemblies[fullName] = assembly;
        }
    }
}
