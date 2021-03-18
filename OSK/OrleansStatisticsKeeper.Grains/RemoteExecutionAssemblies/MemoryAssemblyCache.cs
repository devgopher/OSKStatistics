using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Utils.Client;
using Utils.Crypto;

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
            if (!_assemblies.ContainsKey(assembly.FullName))
                _assemblies[assembly.FullName] = assembly;
        }

        public void Update(Assembly assembly)
        {
            if (_assemblies.ContainsKey(assembly.FullName))
            {
                var existingAsm = Get(assembly.FullName);
                if (!HashUtils.CompareHashes(HashUtils.HashForAssembly(assembly), HashUtils.HashForAssembly(existingAsm))) 
                {
                    // yeah, I think we need a lock, 'cause of we need to use the same copy of an assembly for all grains simultaneouly!
                    lock (_assemblies) 
                        _assemblies[assembly.FullName] = assembly;
                }
            }
        }
    }
}
