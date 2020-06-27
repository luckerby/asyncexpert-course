using System;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            FibonacciCalc myFibonaci = new FibonacciCalc();
            
            
            //Console.WriteLine(myFibonaci.Recursive(35));
            //Console.WriteLine(myFibonaci.RecursiveWithMemoizationStarter(35));
            //Console.WriteLine(myFibonaci.IterativeWithArrays(35));
            //Console.WriteLine(myFibonaci.Iterative(35));

            //Console.WriteLine(myFibonaci.noCalls);

            BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
        }
    }
}
