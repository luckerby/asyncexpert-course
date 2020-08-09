using System;
using System.Diagnostics;
using System.Threading;

namespace Synchronization.Core
{
    /*
     * Implement very simple wrapper around Mutex or Semaphore (remember both implement WaitHandle) to
     * provide a exclusive region created by `using` clause.
     *
     * Created region may be system-wide or not, depending on the constructor parameter.
     */

    // There's a requirement for system-wide as well, so SemaphoreSlim is out
    // of the question.
    public class NamedExclusiveScope : IDisposable
    {
        private Semaphore _semaphore;

        public NamedExclusiveScope(string name, bool isSystemWide)
        {
            if (!isSystemWide)
            {
                // Append something particular to this process only. The PID will do
                name += $"-{Process.GetCurrentProcess().Id}";
            }

            _semaphore = new Semaphore(1, 1, name, out var newSemaphoreCreated);
            if (!newSemaphoreCreated)
            {
                // If we're here, then it means that a semaphore with the intended
                //  name already exists
                throw (new System.InvalidOperationException($"Unable to get a global lock {name}."));
            }
            
            // Guard access to the critical section
            _semaphore.WaitOne();
        }

        public void Dispose()
        {
            // It's here that we need to release whatever lock we've used
            _semaphore.Release();
        }
    }
}
