using Orleans;
using OrleansStatisticsKeeper.Grains.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OrleansStatisticsKeeper.Grains.Grains
{
    public class GenericExecutiveGrain : Grain, IExecutiveGrain
    {
        public bool IsLoaded { get; private set; }
        private Assembly assembly;

        public GenericExecutiveGrain()
        {
        }

        public void LoadAssembly(string asmPath)
        {
            try
            {
                if (asmPath == default)
                {
                    IsLoaded = false;
                    return;
                }

                assembly = Assembly.ReflectionOnlyLoadFrom(asmPath);
                IsLoaded = true;
            }
            catch (Exception)
            {
                IsLoaded = false;
            }
        }

        public void LoadAssembly(byte[] asmBytes)
        {
            try
            {
                if (asmBytes == default || !asmBytes.Any())
                {
                    IsLoaded = false;
                    return;
                }

                assembly = Assembly.ReflectionOnlyLoad(asmBytes);
                IsLoaded = true;
            } catch (Exception)
            {
                IsLoaded = false;
            }
        }

        public async Task<TOUT> Execute<TOUT>(string className, string funcName,
            params object[] args)
        {
            if (!IsLoaded)
                return default;
            var type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == className);

            if (type == default)
                return default;

            MethodInfo method;
            if (args == null)
                method = type.GetMethods().FirstOrDefault(m => m.Name == funcName && m.GetParameters().Length == 0);
            else
                method = type.GetMethods().FirstOrDefault(m => m.Name == funcName && m.GetParameters().Length == args.Length);


            var obj = Activator.CreateInstance(type);

            var ret = method.Invoke(obj, args);

            return (TOUT)ret;
        }
    }
}
