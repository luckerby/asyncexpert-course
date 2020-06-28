using System;
using System.Threading;

namespace ThreadPoolExercises.Core
{
    public class ThreadingHelpers
    {
        //private static AutoResetEvent exit = new AutoResetEvent(false);
        public static void ExecuteOnThread(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            // * Create a thread and execute there `action` given number of `repeats` - waiting for the execution!
            //   HINT: you may use `Join` to wait until created Thread finishes
            // * In a loop, check whether `token` is not cancelled
            // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

            // Use to have the thread action know it's time to finish asap
            bool bail = false;
            // Interval to check if the operation has been cancelled (milliseconds)
            int cancelIntervalCheck_Thread = 100;

            Thread thread = new Thread(() =>
            {
                for (int i = 0; i < repeats; i++)
                {
                    // We can't simply abort the thread externally (.NET Core
                    //  doesn't even support this), so keep an eye out on 
                    //  the signal variable here to stop from within
                    if (bail)
                        break;
                    else
                    {
                        // Can't rely on the AppDomain-defined exception handler,
                        //  since 'ExceptionHandlingWhenErrorActionProvidedTest' will
                        //  fail unless try/catch is used here
                        try
                        {
                            action.Invoke();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Caught an exception within the worked thread");
                            if(errorAction!=null)
                                errorAction.Invoke(e);
                            bail = true;
                        }
                    }
                }
            });

            // Hook the AppDomain event that will catch exceptions from within 'action'. Note
            //  that this won't prevent the app from being killed when an exception occurs. It
            //  also doesn't allow the 'ExceptionHandlingWhenErrorActionProvidedTest' test to
            //  run successfully
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                Console.WriteLine("In AppDomain exception handler");
                Exception exceptionThrown = (Exception)e.ExceptionObject;
                if (errorAction != null)
                    errorAction.Invoke(exceptionThrown);
            };

            thread.Start();
            while (!thread.Join(cancelIntervalCheck_Thread))
            {
                Console.WriteLine('.');
                // Check also that the bail variable hasn't already been set, as we
                //  can still run this twice if calling the error action takes longer
                if (token.IsCancellationRequested && errorAction != null && !bail)
                {
                    bail = true;
                    errorAction.Invoke(new System.OperationCanceledException());
                }
            }
        }

        public static void ExecuteOnThreadPool(Action action, int repeats, CancellationToken token = default, Action<Exception>? errorAction = null)
        {
            // * Queue work item to a thread pool that executes `action` given number of `repeats` - waiting for the execution!
            //   HINT: you may use `AutoResetEvent` to wait until the queued work item finishes
            // * In a loop, check whether `token` is not cancelled
            // * If an `action` throws and exception (or token has been cancelled) - `errorAction` should be invoked (if provided)

            // Use to have the thread action know it's time to finish asap
            bool bail = false;
            // Interval to check if the operation has been cancelled (milliseconds)
            int cancelIntervalCheck_ThreadPool = 100;

            AutoResetEvent finishEvent = new AutoResetEvent(false);
            //ThreadPool.RegisterWaitForSingleObject(finishEvent, Callback, null, -1, true);

            // Convert from 'Action' to 'WaitCallback')
            ThreadPool.QueueUserWorkItem(state => {
                for (int i = 0; i < repeats; i++)
                {
                    if (bail)
                        break;
                    else
                    {
                        try
                        {
                            action.Invoke();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine("Caught an exception within the thread pool's worked thread");
                            if (errorAction != null)
                                errorAction.Invoke(exception);
                            bail = true;
                        }
                    }
                }
                finishEvent.Set();
            });

            // .WaitOne works, but burns the current thread. Any other way ?
            while(!finishEvent.WaitOne(cancelIntervalCheck_ThreadPool) && !bail)
            {
                Console.WriteLine('.');
                // Check also that the bail variable hasn't already been set, as we
                //  can still run this twice if calling the error action takes longer
                if (token.IsCancellationRequested && errorAction != null && !bail)
                {
                    bail = true;
                    errorAction.Invoke(new System.OperationCanceledException());
                }
            }

        }

        /*
        private static void Callback(object? state, bool timedOut)

        {
            Console.WriteLine("Signal received");
            //exit.Set();
        }
        */
    }
}
