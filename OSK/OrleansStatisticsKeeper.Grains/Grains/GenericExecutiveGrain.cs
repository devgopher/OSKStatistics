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
    public class GenericExecutiveGrain : Grain, IOskGrain
    {
        private IAssemblyMembersCache _assemblyMembersCache;
        private readonly IAsyncLogger _logger;

        public GenericExecutiveGrain(IAssemblyMembersCache assemblyMembersCache, IAsyncLogger logger)
        {
            _assemblyMembersCache = assemblyMembersCache;
            _logger = logger;
        }

        public async Task<bool> GetIsLoaded(Type type)
            => _assemblyMembersCache.GetAssemblyForType(type) != default;

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
                    _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() asmPath = null!");
                    return;
                }

                if (!_assemblyMembersCache.AssemblyExists(assemblyFullName))
                {
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" in assembly cache... trying to add a new one...");
                    var assembly = Assembly.LoadFrom(asmPath);
                    _assemblyMembersCache.AddAssembly(assembly);
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" added into a cache");
                }
               
                _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() assembly loaded");
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() exception", ex);
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

                    return;
                }

                if (!_assemblyMembersCache.AssemblyExists(assemblyFullName))
                {
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" in assembly cache... trying to add a new one...");
                    var assembly = Assembly.Load(asmBytes);
                    _assemblyMembersCache.AddAssembly(assembly);
                    _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() no assembly '{assemblyFullName}'" +
                        $" added into a cache");
                }
      
                _logger.Info($"{this.GetType().Name}.{nameof(LoadAssembly)}() assembly loaded");

            } catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name}.{nameof(LoadAssembly)}() exception", ex);
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

            var assembly = _assemblyMembersCache.GetAssemblyForType(className);

            var type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == className);

            if (type == default)
                return default;

            if (!await GetIsLoaded(type))
                throw new GrainException($"Nothing to execute!");

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

        public async Task<TOUT> ExecuteWithContext<TOUT>(string className, string funcName, object context, params object[] args)
        {
            _logger.Info($"{this.GetType().Name}.{nameof(Execute)}() started...");

            var assembly = _assemblyMembersCache.GetAssemblyForType(className);

            var type = assembly.DefinedTypes.FirstOrDefault(t => t.Name == className);

            if (type == default)
                return default;

            if (!await GetIsLoaded(type))
                throw new GrainException($"Nothing to execute!");

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

            var obj = Activator.CreateInstance(type, context);
            _logger.Trace($"{this.GetType().Name}.{nameof(Execute)}() instance created");

            var ret = method.Invoke(obj, args);
            _logger.Trace($"{this.GetType().Name}.{nameof(Execute)}() method invoked and we've got a result");

            return (TOUT)ret;
        }

    }
}