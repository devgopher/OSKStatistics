using AsyncLogging;
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
    /// <summary>
    /// This type of grain is intended for a remote method execution
    /// </summary>
    public class GenericExecutiveGrain : Grain, IExecutiveGrain
    {
        private bool isLoaded;
        public async Task<bool> GetIsLoaded() => isLoaded;
        public async Task SetIsLoaded(bool val) => isLoaded = val;

        private Assembly assembly;
        private IAssemblyCache _assemblyCache;
        private readonly IAsyncLogger _logger;

        public GenericExecutiveGrain(IAssemblyCache assemblyCache, IAsyncLogger logger)
        {
            _assemblyCache = assemblyCache;
            _logger = logger;
        }

        /// <summary>
        /// Loading an assembly with required class and method
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="version"></param>
        /// <param name="asmPath"></param>
        /// <returns></returns>
        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, string asmPath)
        {
            try
            {
                _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() started...");
                if (asmPath == default)
                {
                    _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() asmpath = null!");
                    await SetIsLoaded(false);
                    return;
                }

                if (!_assemblyCache.Exists(assemblyFullName))
                {
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" in assembly cache... trying to add a new one...");
                    assembly = Assembly.LoadFrom(asmPath);
                    _assemblyCache.Set(assembly);
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" added into a cache");
                }
                else
                {
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() assembly '{assemblyFullName}'" +
                        $" is already in assembly cache");
                    assembly = _assemblyCache.Get(assemblyFullName);
                }

                _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() assembly loaded");
                await SetIsLoaded(true);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() exception", ex);
                await SetIsLoaded(false);
            }
        }

        /// <summary>
        /// Loading an assembly with required class and method
        /// </summary>
        /// <param name="assemblyFullName"></param>
        /// <param name="version"></param>
        /// <param name="asmBytes"></param>
        /// <returns></returns>
        public async Task LoadAssembly(string assemblyFullName, FileVersionInfo version, byte[] asmBytes)
        {
            try
            {
                _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() started...");

                if (asmBytes == default || !asmBytes.Any())
                {
                    _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() asmBytes = null or empty!");

                    await SetIsLoaded(false);
                    return;
                }

                if (!_assemblyCache.Exists(assemblyFullName))
                {
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" in assembly cache... trying to add a new one...");
                    assembly = Assembly.Load(asmBytes);
                    _assemblyCache.Set(assembly);
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" added into a cache");
                }
                else
                {
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() assembly '{assemblyFullName}'" +
                                 $" is already in assembly cache");
                    assembly = _assemblyCache.Get(assemblyFullName);
                }

                _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() assembly loaded");
                await SetIsLoaded(true);

            } catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() exception", ex);
                await SetIsLoaded(false);
            }
        }

        /// <summary>
        /// Executes a function for a class
        /// </summary>
        /// <typeparam name="TOUT"></typeparam>
        /// <param name="className"></param>
        /// <param name="funcName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<TOUT> Execute<TOUT>(string className, string funcName,
            params object[] args)
        {
            _logger.Info($"{this.GetType().Name}.{nameof(Execute)}() started...");
            if (!await GetIsLoaded()) 
                throw new GrainException($"Nothing to execute!");

            var type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == className);

            if (type == default)
                return default;

            _logger.Info($"{this.GetType().Name}.{nameof(Execute)}() type/method: {type.Name}/{funcName}");

            MethodInfo method;
            if (args == default)
                method = type.GetDeclaredMethods(funcName).FirstOrDefault(m => m.GetParameters().Length == 0);
            else
                method = type.GetDeclaredMethods(funcName).FirstOrDefault(m => m.GetParameters().Length == args.Length);

            if (method == default)
                throw new GrainException($"Can't find a method with name {funcName}!");

            _logger.Trace($"{this.GetType().Name}.{nameof(Execute)}() method was found: {method.Name}");


            if (method.IsStatic)
                return (TOUT)method.Invoke(null, args);

            var obj = Activator.CreateInstance(type);
            _logger.Trace($"{this.GetType().Name}.{nameof(Execute)}() instance created");

            var ret = method.Invoke(obj, args);
            _logger.Trace($"{this.GetType().Name}.{nameof(Execute)}() method invoked and we've got a result");

            return (TOUT)ret;
        }
    }
}