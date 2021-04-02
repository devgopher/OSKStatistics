using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.Process;

namespace OrleansStatisticsKeeper.SiloHost.Utils
{
    class CpuOptimizer
    {
        private const int max_thread_count = 1000;
        private const int min_thread_count = 10;
        private static int current_thread_count = 50;

        public static async Task Start(int percent, CancellationToken cancellationToken)
        {
            ThreadPool.SetMaxThreads(max_thread_count, 2 * max_thread_count);
            ThreadPool.SetMinThreads(min_thread_count, 2 * min_thread_count);

            if (percent < 0 || percent > 100)
                throw new InvalidOperationException("Percent must be in a diapazone [0..100]!");

            using (var cpuCounter = ProcessCpuCounter.GetPerfCounterForProcessId(Process.GetCurrentProcess().Id))
            {
                while (/*!cancellationToken.IsCancellationRequested*/true)
                {
                    var cpuValue = cpuCounter.NextValue();
                    Console.WriteLine($"CPU USAGE: {cpuValue}: {percent}!");
                    if (cpuValue > percent)
                    {
                        Console.WriteLine($"CPU BOUNDS EXCEEDED: {cpuValue}: {percent}!");
                        while (current_thread_count > min_thread_count && cpuValue > percent)
                        {
                            int delta = (int)(0.1 * current_thread_count);
                            delta = delta > 1 ? delta : 1;
                            current_thread_count -= delta;
                            current_thread_count = current_thread_count > min_thread_count ? current_thread_count : min_thread_count;
                            ThreadPool.SetMaxThreads(current_thread_count, 2 * current_thread_count);
                            Thread.Sleep(500);
                            cpuValue = cpuCounter.NextValue();
                        }
                    }
                    else if (cpuCounter.NextValue() < 0.5 * percent)
                    {
                        Console.WriteLine($"CPU IS UNDERUSED: {cpuValue}: {percent}!");
                        while (current_thread_count > min_thread_count && cpuCounter.NextValue() < percent)
                        {
                            int delta = (int)(0.1 * current_thread_count);
                            delta = delta > 1 ? delta : 1;
                            current_thread_count += delta;
                            current_thread_count = current_thread_count < max_thread_count ? current_thread_count : max_thread_count;
                            ThreadPool.SetMaxThreads(current_thread_count, 2 * current_thread_count);
                            Thread.Sleep(500);
                            cpuValue = cpuCounter.NextValue();
                        }
                    }

                    Console.WriteLine($"CURRENT THREAD COUNT: {current_thread_count}");
                    Thread.Sleep(15000);
                }
            }
        }
    }
}
