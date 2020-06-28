using System;
using System.Threading;
using ThreadPoolExercises.Core;

namespace ThreadPoolExercises
{
    class Program
    {
        static void Main(string[] args)
        {
            // Here you can play around with those method, prototype and easily debug

            Console.WriteLine($"Main thread is {Thread.CurrentThread.ManagedThreadId}");

            // Generate a token that's immediately cancelled
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel();
            CancellationToken cancelToken = cts.Token;

            // new CancellationTokenSource(200).Token
            /*
            ThreadingHelpers.ExecuteOnThread(() =>
            {
                var thread = Thread.CurrentThread;
                Console.WriteLine($"Hello from thread {thread.ManagedThreadId} from a pool: {thread.IsThreadPoolThread}");
                Thread.Sleep(1000);
                //throw new StackOverflowException();
            }, 3, cancelToken,
            (System.Exception exception) => { Console.WriteLine($"<{exception.Message}> occurred !"); });
            */

            /*
            ThreadingHelpers.ExecuteOnThread(
                () => throw new NullReferenceException(),
                10,
                new CancellationTokenSource(1000).Token,
                errorAction: (System.Exception exception) => { Console.WriteLine($"<{exception.Message}> occurred !"); });
            */

            /*
            ThreadingHelpers.ExecuteOnThreadPool(() =>
            {
                var thread = Thread.CurrentThread;
                Console.WriteLine(
                    $"Hello from thread {thread.ManagedThreadId} from a pool: {thread.IsThreadPoolThread}");
            }, 3);
            */

            
            ThreadingHelpers.ExecuteOnThreadPool(
                () => Thread.Sleep(100),
                3,
                cancelToken,
                errorAction: (System.Exception exception) => { Console.WriteLine($"<{exception.Message}> occurred !"); }
            );
            
        }
    }
}
