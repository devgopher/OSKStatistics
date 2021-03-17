using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Utils.Client
{
    public static class AssemblyUtils
    {
        public static string GetAssemblyPath(Type type)
            => type.Assembly.Location;

        public static byte[] GetAssemblyBinary(string path)
            => File.ReadAllBytes(path);

        public static byte[] GetAssemblyBinary(Type type)
            => File.ReadAllBytes(type.Assembly.Location);

        public static byte[] GetAssemblyBinary(Assembly assembly)
            => File.ReadAllBytes(assembly.Location);

        public static string GetAssemblyName(Type type)
            => type.Assembly.FullName;


        public static string GetAssemblyName(string path)
            => Assembly.LoadFile(path)?.FullName;

        public static FileVersionInfo GetAssemblyVersion(Type type)
            => FileVersionInfo.GetVersionInfo(type.Assembly.Location);

        public static FileVersionInfo GetAssemblyVersion(Assembly assembly)
            => GetAssemblyVersion(assembly.Location);

        public static FileVersionInfo GetAssemblyVersion(string path)
            => FileVersionInfo.GetVersionInfo(path);
    }
}
