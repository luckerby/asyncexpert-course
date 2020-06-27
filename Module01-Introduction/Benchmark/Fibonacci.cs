using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Dotnetos.AsyncExpert.Homework.Module01.Benchmark
{
    [DisassemblyDiagnoser(exportCombinedDisassemblyReport: true)]
    [MemoryDiagnoser]
    //[NativeMemoryProfiler]
    //[SimpleJob(launchCount:1,targetCount:1)]
    public class FibonacciCalc
    {
        private ulong[] fibonacci;// = new ulong[1001];
        public ulong noCalls = 0;
        // HOMEWORK:
        // 1. Write implementations for RecursiveWithMemoization and Iterative solutions
        // 2. Add MemoryDiagnoser to the benchmark
        // 3. Run with release configuration and compare results
        // 4. Open disassembler report and compare machine code
        // 
        // You can use the discussion panel to compare your results with other students

        /*
        [GlobalSetup]
        public void CreateMemoizationStructure()
        {
            fibonacci = new ulong[1001];
        }
        */

        //[Benchmark(Baseline = true)]
        //[ArgumentsSource(nameof(Data))]
        public ulong Recursive(ulong n)
        {
            noCalls++;
            if (n == 1 || n == 2) return 1;
            return Recursive(n - 2) + Recursive(n - 1);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoizationStarter(ulong n)
        {
            fibonacci = new ulong[n + 1];
            return RecursiveWithMemoization(n);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public ulong RecursiveWithMemoizationStarterWithParam(ulong n)
        {
            fibonacci = new ulong[n + 1];
            return RecursiveWithMemoization(n, fibonacci);
        }

        public ulong RecursiveWithMemoization(ulong n, ulong[] fibonacci)
        {
            if (n == 1 || n == 2) {
                fibonacci[n] = 1;
                return 1;
            }
            if (fibonacci[n] != 0)
                return fibonacci[n];
            else
            {
                if (fibonacci[n - 2] == 0)
                {
                    fibonacci[n - 2] = RecursiveWithMemoization(n - 2, fibonacci);
                }
                if (fibonacci[n - 1] == 0)
                {
                    fibonacci[n - 1] = RecursiveWithMemoization(n - 1, fibonacci);
                }
                fibonacci[n] = fibonacci[n - 2] + fibonacci[n - 1];
                return fibonacci[n];
            }
        }

        public ulong RecursiveWithMemoization(ulong n)
        {
            if (n == 1 || n == 2)
            {
                fibonacci[n] = 1;
                return 1;
            }
            if (fibonacci[n] != 0)
                return fibonacci[n];
            else
            {
                if (fibonacci[n - 2] == 0)
                {
                    fibonacci[n - 2] = RecursiveWithMemoization(n - 2);
                }
                if (fibonacci[n - 1] == 0)
                {
                    fibonacci[n - 1] = RecursiveWithMemoization(n - 1);
                }
                fibonacci[n] = fibonacci[n - 2] + fibonacci[n - 1];
                return fibonacci[n];
            }
        }


        //[Benchmark]
        //[ArgumentsSource(nameof(Data))]
        public ulong IterativeWithArrays(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            fibonacci[1] = 1;
            fibonacci[2] = 1;
            for (ulong i = 3; i <= n; i++)
                fibonacci[i] = fibonacci[i - 2] + fibonacci[i - 1];
            return fibonacci[n];
        }

        //[Benchmark]
        //[ArgumentsSource(nameof(Data))]
        public ulong Iterative(ulong n)
        {
            if (n == 1 || n == 2) return 1;
            ulong firstNumber = 1;
            ulong secondNumber = 1;
            for (ulong i = 3; i <= n; i++)
            {
                ulong newNumber = firstNumber + secondNumber;
                firstNumber = secondNumber;
                secondNumber = newNumber;
            }
                
            return secondNumber;
        }

        public IEnumerable<ulong> Data()
        {
            //yield return 1;
            //yield return 2;
            yield return 5;
            yield return 20;
            yield return 35;
        }


    }
}
