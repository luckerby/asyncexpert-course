using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitExercises.Core
{
    public class AsyncHelpers
    {
        public static async Task<string> GetStringWithRetries(HttpClient client, string url, int maxTries = 3, CancellationToken token = default)
        {
            // Create a method that will try to get a response from a given `url`, retrying `maxTries` number of times.
            // It should wait one second before the second try, and double the wait time before every successive retry
            // (so pauses before retries will be 1, 2, 4, 8, ... seconds).
            // * `maxTries` must be at least 2
            // * we retry if:
            //    * we get non-successful status code (outside of 200-299 range), or
            //    * HTTP call thrown an exception (like network connectivity or DNS issue)
            // * token should be able to cancel both HTTP call and the retry delay
            // * if all retries fails, the method should throw the exception of the last try
            // HINTS:
            // * `HttpClient.GetAsync` does not accept cancellation token (use `GetAsync` instead)
            // * you may use `EnsureSuccessStatusCode()` method

            if (maxTries < 2)
                throw new ArgumentException();

            Exception lastAttemptException = null;
            string payloadData = null;                  // the payload of the retrieved page
            bool currentRequestFailed = false;
            HttpResponseMessage response = null;
            int noOfWaitsSoFar = 0;
            do
            {
                currentRequestFailed = false;
                try
                {
                    // Create a new response every time, to ensure we don't
                    //  reuse a former value. GC should pick up the former ones
                    response = new HttpResponseMessage();
                    Console.WriteLine("Issuing GetAsync");
                    response = await client.GetAsync(url, token);
                    Console.WriteLine("GetAsync done");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception fired");
                    currentRequestFailed = true;
                    lastAttemptException = e;
                }
                if (!response.IsSuccessStatusCode)
                    currentRequestFailed = true;
                if (!currentRequestFailed)
                    payloadData = await response.Content.ReadAsStringAsync();

                // Wait the required number of seconds
                if(currentRequestFailed)
                {
                    int noMillisecondsToWait = (1 << noOfWaitsSoFar) * 1000;
                    Console.WriteLine($"Waiting {noMillisecondsToWait} ms");
                    await Task.Delay(noMillisecondsToWait, token);
                    noOfWaitsSoFar++;
                }

            } while (currentRequestFailed && noOfWaitsSoFar<maxTries);

            if (!currentRequestFailed)
                return payloadData;
            else
                throw lastAttemptException ?? new HttpRequestException();
        }

    }
}
