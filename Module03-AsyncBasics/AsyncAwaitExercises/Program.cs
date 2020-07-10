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
        static async Task Main(string[] args)
        {
            CancellationToken cancelToken = new CancellationTokenSource(6000).Token;
            // Here you can play around with those method, prototype and easily debug
            using var client = new HttpClient();

            try
            {
                DumpThread("Before call");
                var result = await AsyncHelpers.GetStringWithRetries(client, "https://postman-echo.com/status/200", token:cancelToken);
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
