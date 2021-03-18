using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies
{
    public class MemoryAssemblyMembersCache : IAssemblyMembersCache
    {
        private readonly IAssemblyCache _assemblyCache;
        private readonly ConcurrentDictionary<string, Assembly> _innerCache
            = new ConcurrentDictionary<string, Assembly>();

        public MemoryAssemblyMembersCache(IAssemblyCache assemblyCache) => _assemblyCache = assemblyCache;

        public static string GetFullKey(Type type) => $"{type.Assembly.GetName()}.{type.Name}";

        public void AddAssembly(Assembly assembly)
        {
            if (!_assemblyCache.Exists(assembly.FullName))
                _assemblyCache.Set(assembly);

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var fullKey = GetFullKey(type);
                _innerCache[fullKey] = assembly;
            }
        }

        public bool AssemblyExists(string assemblyFullName)
            => _assemblyCache.Exists(assemblyFullName);

        public Assembly GetAssembly(string assemblyFullName)
            => _assemblyCache.Get(assemblyFullName);

        public Assembly GetAssemblyForType(Type type)
            => _assemblyCache.Get(type.Assembly.FullName);

        public Assembly GetAssemblyForType(string type)
        {
            if (!_innerCache.Any(t => t.Key.Contains($".{type}")))
                return null;
            var kvp = _innerCache.FirstOrDefault(t => t.Key.Contains($".{type}"));
            return  kvp.Value;
        }
    }
}
