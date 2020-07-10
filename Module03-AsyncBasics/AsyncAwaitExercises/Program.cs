using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitExercises.Core;

namespace AsyncAwaitExercises
{
    class Program
    {
        static Task<int> ReturnOne()
        {
            //return 1;
            return Task<int>.FromResult(1);
        }

        static void/*int*/ ReturnOneSimple()
        {
            //return 1;
        }

        static async Task Main(string[] args)
        {
            //for (int i = 0; i < 10; i++)
            //    Task.Run(() => Console.WriteLine(i));

            //Task.Delay(1000);
            //Task T = ReturnOne();    // how does this one work ?
            Task<int> T = new Task(new Action<(ReturnOneSimple));  //fails to compile, but why ?
            //Task<int> T = Task.Run(ReturnOne);

            //T.Wait();

            Task.Delay(1000).Wait();
            return;

            // Here you can play around with those method, prototype and easily debug
            using var client = new HttpClient();

            try
            {
                DumpThread("Before call");
                var result = await AsyncHelpers.GetStringWithRetries(client, "https://postman-echo.com/status/500");
                DumpThread("After call");

            }
            catch (Exception ex)
            {
                DumpThread($"After exception {ex}");
            }
        }

        static void DumpThread(string label) =>
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] {label}: TID:{Thread.CurrentThread.ManagedThreadId}");
    }
}
