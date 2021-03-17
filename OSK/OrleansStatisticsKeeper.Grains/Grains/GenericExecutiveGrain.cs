using Orleans;
using OrleansStatisticsKeeper.Grains.Exceptions;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.RemoteExecutionAssemblies;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class GenericExecutiveGrain : Grain, IExecutiveGrain
    {
        private bool isLoaded;
        public async Task<bool> GetIsLoaded() => isLoaded;
        public async Task SetIsLoaded(bool val) => isLoaded = val;

        private Assembly assembly;
        private IAssemblyCache _assemblyCache;

        public GenericExecutiveGrain(IAssemblyCache assemblyCache) => _assemblyCache = assemblyCache;

        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, string asmPath)
        {
            try
            {
                if (asmPath == default)
                {
                    await SetIsLoaded(false);
                    return;
                }

                if (!_assemblyCache.Exists(assemblyFullName))
                {
                    assembly = Assembly.LoadFrom(asmPath);
                    _assemblyCache.Set(assembly);
                }
                else
                    assembly = _assemblyCache.Get(assemblyFullName);

                await SetIsLoaded(true);
            }
            catch (Exception ex)
            {
                await SetIsLoaded(false);
            }
        }

        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, byte[] asmBytes)
        {
            try
            {
                if (asmBytes == default || !asmBytes.Any())
                {
                    await SetIsLoaded(false);
                    return;
                }

                if (!_assemblyCache.Exists(assemblyFullName))
                {
                    assembly = Assembly.Load(asmBytes);
                    _assemblyCache.Set(assembly);
                }
                else
                    assembly = _assemblyCache.Get(assemblyFullName);
                await SetIsLoaded(true);

            } catch (Exception ex)
            {
                await SetIsLoaded(false);
            }
        }

        public async Task<TOUT> Execute<TOUT>(string className, string funcName,
            params object[] args)
        {
            if (!await GetIsLoaded())
                throw new GrainException($"Nothing to execute!");
            var type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == className);

            if (type == default)
                return default;

            MethodInfo method;
            if (args == default)
                method = type.GetDeclaredMethods(funcName).FirstOrDefault(m => m.GetParameters().Length == 0);
            else
                method = type.GetDeclaredMethods(funcName).FirstOrDefault(m => m.GetParameters().Length == args.Length);

            if (method == default)
                throw new GrainException($"Can't find a method with name {funcName}!");

            if (method.IsStatic)
                return (TOUT)method.Invoke(null, args);

            var obj = Activator.CreateInstance(type);
            var ret = method.Invoke(obj, args);

            return (TOUT)ret;
        }
    }
}