using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskCompletionSourceExercises.Core
{
    public class AsyncTools
    {
        public static Task<string> RunProgramAsync(string path, string args = "")
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            
            // all the staff below needs to be wrapped in a TaskCompletionSource
            Task.Run(() =>
            {
                try
                {
                    var process = new Process();
                    process.EnableRaisingEvents = true;
                    process.StartInfo = new ProcessStartInfo(path, args)
                    {
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    process.Exited += (sender, eventArgs) =>
                    {
                        var senderProcess = sender as Process;
                        Console.WriteLine("----- program output -----");
                        Console.WriteLine($"Exit code: {senderProcess?.ExitCode}");
                        Console.WriteLine("----- program output -----");

                        // result - if no exception - is:
                        if (senderProcess?.ExitCode == 0)
                            tcs.SetResult(senderProcess?.StandardOutput.ReadToEnd());
                        else
                        {
                            tcs.SetException(new Exception(senderProcess?.StandardError.ReadToEnd()));
                        }

                        senderProcess?.Dispose();
                    };
                    process.Start();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }
    }
}
