namespace OrleansTestApplication
{
    class Program
    {
        public static int Main(string[] args) =>  (int)(new TestRemoteExecution()).RunMainAsync().Result;

    }
}
