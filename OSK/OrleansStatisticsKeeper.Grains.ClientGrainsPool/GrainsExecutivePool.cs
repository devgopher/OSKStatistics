using Ceras;
using Newtonsoft.Json;
using OrleansStatisticsKeeper.Client;
using OrleansStatisticsKeeper.Grains.Interfaces;
using OrleansStatisticsKeeper.Grains.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OrleansStatisticsKeeper.Grains.ClientGrainsPool
{
    [Serializable]
    public class GrainsExecutivePool : GrainsPool<IExecutiveGrain>
    {
        private readonly SerializerConfig serializerConfig = new SerializerConfig();
        private readonly CerasSerializer _ceras = new CerasSerializer();

        public GrainsExecutivePool(StatisticsClient client, int poolSize) : base(client, poolSize)
        {
            serializerConfig.Advanced.DelegateSerialization = DelegateSerializationFlags.AllowInstance;
            _ceras = new CerasSerializer(serializerConfig);
        }

        public Task<TOUT> Execute<TOUT>(Func<TOUT> func)
        {
            using (var ms = new MemoryStream())
            {
                var ser = _ceras.Serialize(func);
                var ret = GetGrain().Execute<TOUT>(ser);
                return ret;
            }
        }
        public Task<TOUT> Execute<TIN1, TOUT>(Func<TIN1, TOUT> func, TIN1 in1)
        {
            using (var ms = new MemoryStream())
            {
                var ser = _ceras.Serialize(func);
                Console.WriteLine($"ser.Length: {ser.Length}");

                var ret = GetGrain().Execute<TOUT>(ser, in1);
                return ret;
            }
        }

        public Task<TOUT> Execute<TIN1, TIN2, TOUT>(Func<TIN1, TIN2, TOUT> func, TIN1 in1, TIN2 in2)
            where TOUT : class
        {
            using (var ms = new MemoryStream())
            {
                var ser = _ceras.Serialize(func);
                var ret = GetGrain().Execute<TOUT>(ser, in1, in2);
                return ret;
            }
        }

        public Task<TOUT> Execute<TIN1, TIN2, TIN3, TOUT>(Func<TIN1, TIN2, TIN3, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3)
            where TOUT : class
        {
            using (var ms = new MemoryStream())
            {
                var ser = _ceras.Serialize(func);
                var ret = GetGrain().Execute<TOUT>(ser, in1, in2, in3);
                return ret;
            }
        }

        public Task<TOUT> Execute<TIN1, TIN2, TIN3, TIN4, TOUT>(Func<TIN1, TIN2, TIN3, TIN4, TOUT> func, TIN1 in1, TIN2 in2, TIN3 in3, TIN4 in4)
            where TOUT : class
        {
            using (var ms = new MemoryStream())
            {
                var ser = _ceras.Serialize(func);
                var ret = GetGrain().Execute<TOUT>(ser, in1, in2, in3, in4);
                return ret;
            }
        }
    }
}