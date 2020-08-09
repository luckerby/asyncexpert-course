using System;
using System.Diagnostics;
using System.Threading;
using Synchronization.Core;

namespace Synchronization
{
    class Program
    {
        static void Main(string[] args)
        {
            var scopeName = "default";
            var isSystemWide = false;
            if (args.Length == 2)
            {
                scopeName = args[0];
                isSystemWide = bool.Parse(args[1]);
            }

            /*
            Thread thread1 = new Thread(MyMethod);
            Thread thread2 = new Thread(MyMethod);
            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();
            */

            using (new NamedExclusiveScope(scopeName, isSystemWide))
            {
                Console.WriteLine("Hello world!");
                Thread.Sleep(300);
            }
        }

        static void MyMethod()
        {
            using (new NamedExclusiveScope("default", false))
            {
                Console.WriteLine($"Hello world from thread: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(3000);
            }
        }
    }
}
