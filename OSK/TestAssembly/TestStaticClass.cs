using System;

namespace TestAssembly
{
    public static class TestStaticClass
    {
        public static double Pow2(int a)
            => Math.Pow(a, 2);

        public static double Pow3(int a)
            => Math.Pow(a, 3);
    }
}
