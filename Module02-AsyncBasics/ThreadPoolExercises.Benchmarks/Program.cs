using System;
using BenchmarkDotNet.Running;

namespace ThreadPoolExercises.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadingHelpersBenchmarks benchmarkInstance = new ThreadingHelpersBenchmarks();
            benchmarkInstance.Setup();
            
            /*
            for (int i = 0; i < 65536; i++)
                benchmarkInstance.ExecuteSynchronously();
            */
            
            for (int i = 0; i < 65536; i++)
                benchmarkInstance.ExecuteOnThread();
            
            
            /*
            for (int i = 0; i < 65536; i++)
                benchmarkInstance.ExecuteOnThreadPool();
            */


            //BenchmarkRunner.Run<ThreadingHelpersBenchmarks>();
        }
    }
}
