using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Utils.Client
{
    public static class ClientAssemblyUtils
    {
        public static string GetAssemblyPath(Type type)
            => type.Assembly.Location;

        public static byte[] GetAssemblyBinary(Type type)
        {
            var path = GetAssemblyPath(type);
            var bytes = File.ReadAllBytes(path);
            return bytes;
        }
    }
}
