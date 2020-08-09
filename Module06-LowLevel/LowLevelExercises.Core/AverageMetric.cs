using System.Threading;

namespace LowLevelExercises.Core
{
    /// <summary>
    /// A simple class for reporting a specific value and obtaining an average.
    /// </summary>
    /// TODO: remove the locking and use <see cref="Interlocked"/> and <see cref="Volatile"/> to implement a lock-free implementation.
    public class AverageMetric
    {
        // TODO: this should not be needed, once you remove all the locks below
        readonly object sync = new object();

        int sum = 0;
        int count = 0;

        public void ReportWithLock(int value)
        {
            // TODO: how to increment sum + count without locking?
            lock (sync)
            {
                sum += value;
                count += 1;
            }
        }

        public void ReportCareless(int value)
        {
            // We simply increment, without any care about multiple threads accessing
            //  the critical section. The result will be that the both sums computed
            //  ('sum' and 'count') will be less than the correct values, due to the
            //  fact that registers will most likely be used to compute the intermediary
            //  values, then stored back in the global memory value, resulting in an
            //  increment done by 2 threads to only be reflected as one back in the
            //  global memory value
            sum += value;
            count += 1;
        }

        public void Report(int value)
        {
            /* 
             The original code can be broken as such:

            (1) int tempSum = sum;
            (2) tempSum = tempSum + value;
            (3) sum = tempSum;
            (4) int newCount = count;
            (5) newCount = newCount + 1;
            (6) count = newCount;

            The following operations are seen:
            - In 1: a LOAD (reading sum), followed by a STORE (writing tempSum)
            - In 2: a LOAD (reading value), followed by a STORE (writing tempSum)
            - In 3: a STORE (writing sum)
            - In 4: a LOAD (reading count), followed by a STORE (writing newCount)
            - In 5: a STORE (writing newCount)
            - In 6: a STORE (writing count)
            
            Would reordering be a problem with these 6 lines ? The compiler/CPUs
            should only reorder code if it has no impact on the code logic, as seen
            from a single-threaded perspective. So within the same CPU,
            we wouldn't expect to see line 2 moved before 1, as that would break the logic consistency; but we
            might very well see the first 3 lines interspersed with the next 3, but keeping their relative order.
            From a hardware perspective at least, on x86 this shouldn't be a problem.
            !But what if the store buffers are not flushed (what write barriers would handle ?). Consider working
            with the cache line directly, as opposed to a register. LE: Running on the same CPU will cause it to
            snoop within its own store buffer (most likely ?) and use that value for subsequent increments. As such
            it doesn't matter whether the store buffers get flushed on this CPU, but only from the perspective of
            other CPUs running the same thread, which get to see a stale value of the cache line for the global
            variable representing the counter

            But we do have an issue with multiple threads running this same critical section at the same time.
            Why ? Because we can miss updates to the 2 variables performed by multiple threads: consider count.
            2 threads, running each on a separate CPU, read the content of the same memory address of the global
            'count' variable independently into one of their registers, then each increment their respective register, and
            then write back sequentially to the original memory address of 'count'. The problem is that each thread will
            write back the same value, thereby missing an unit in the overall increment: instead of adding 2 to 'count',
            the result is that only 1 is added.
            So riddling the code with Interlocked.MemoryBarrier will not help from the perspective of getting the 
            correct answer. Instead we need to lock the operation itself, with Interlocked.Add/Increment. What changes
            when using these instructions ? The assembly instruction (for x86 architecture) will have a LOCK prefix.
            This answer https://stackoverflow.com/a/39396999/5853218 explains that atomicity here for the whole
            read-modify-write is achieved by the respective CPU keeping the cache line belonging to the global variable 
            in a modified state for the whole duration of the read (and not only when writing, as it happens when read-to-register-then-update scenario)
             
            */

            Interlocked.Add(ref sum, value);
            Interlocked.Increment(ref count);

        }

        public double Average
        {
            get
            {
                // TODO: how to access the values in a lock-free way?
                // let's assume that we can return value estimated on a bit stale data(in time average will be less and less diverged)
                lock (sync)
                {
                    return Calculate(count, sum);
                }
            }
        }

        static double Calculate(in int count, in int sum)
        {
            // DO NOT change the way calculation is done.

            if (count == 0)
            {
                return double.NaN;
            }

            return (double)sum / count;
        }
    }
}
