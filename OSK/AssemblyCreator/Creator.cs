using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace AssemblyCreator
{
    public static class Creator
    {
        public Assembly Create(string name)
        {
            var asm = new Assembly.CreateInstance();
        }

        public CodeGenerator()
        {
            AppDomain currentDomain = Thread.GetDomain();

            AssemblyName assemName = new AssemblyName
            {
                Name = "MyAssembly"
            };

            var assemBuilder = AssemblyBuilder.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.RunAndCollect);

            ModuleBuilder moduleBuilder = assemBuilder.DefineDynamicModule("MyModule");

            TypeBuilder typeBuilder = moduleBuilder.DefineType("MyClass", TypeAttributes.Public);

            MethodBuilder methodBuilder = typeBuilder.DefineMethod("HelloWorld", MethodAttributes.Public, null, null);

            ILGenerator msilG = methodBuilder.GetILGenerator();
            msilG.EmitWriteLine("www.java2s.com");
            msilG.Emit(OpCodes.Ret);
        }
    }
}
