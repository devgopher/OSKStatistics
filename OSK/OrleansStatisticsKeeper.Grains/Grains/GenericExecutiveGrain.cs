using Orleans;
using OrleansStatisticsKeeper.Grains.Exceptions;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
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

        public GenericExecutiveGrain()
        {
        }

        public async Task LoadAssembly(string asmPath)
        {
            try
            {
                if (asmPath == default)
                {
                    await SetIsLoaded(false);
                    return;
                }

                assembly = Assembly.LoadFrom(asmPath);
                await SetIsLoaded(true);
            }
            catch (Exception ex)
            {
                await SetIsLoaded(false);
            }
        }

        public async Task LoadAssembly(byte[] asmBytes)
        {
            try
            {
                if (asmBytes == default || !asmBytes.Any())
                {
                    await SetIsLoaded(false);
                    return;
                }

                assembly = Assembly.Load(asmBytes);
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