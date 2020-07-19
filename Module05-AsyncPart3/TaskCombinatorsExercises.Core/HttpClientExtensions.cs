using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCombinatorsExercises.Core
{
    public static class HttpClientExtensions
    {
        /*
         Write cancellable async method with timeout handling, that concurrently tries to get data from
         provided urls (first wins and its response is returned, rest is __cancelled__).
         
         Tips:
         * consider using HttpClient.GetAsync (as it is cancellable)
         * consider using Task.WhenAny
         * you may use urls like for testing https://postman-echo.com/delay/3
         * you should have problem with tasks cancellation -
            - how to merge tokens of operations (timeouts) with the provided token? 
            - Tip: you can link tokens with the help of CancellationTokenSource.CreateLinkedTokenSource(token)
         */
        public static async Task<string> ConcurrentDownloadAsync(this HttpClient httpClient,
            string[] urls, int millisecondsTimeout, CancellationToken token)
        {
            // start all the retrieval tasks at the same time
            // for V2, would a IAsyncEnumerable work better here ?
            List<Task> tasks = new List<Task>();
            foreach (var url in urls)
            {
                Task<HttpResponseMessage> t = httpClient.GetAsync(url, token);
                tasks.Add(t);
            }

            Task timeoutTask = Task.Delay(millisecondsTimeout, token);

            // Add the timeout task to the array of tasks, since we cannot put
            //  the task array and one task in the same parameter list for WhenAny
            tasks.Add(timeoutTask);

            Task firstTaskDone = await Task.WhenAny(tasks);
            if(firstTaskDone==timeoutTask || firstTaskDone.IsCanceled)
                throw new TaskCanceledException();
            else
            {
                // Convert the task to the type we want (we know it returns HttpResponseMessage). We
                // also know that the task finished, so we get the Result directly
                HttpResponseMessage fastestResponse = ((Task<HttpResponseMessage>) firstTaskDone).Result;
                return await fastestResponse.Content.ReadAsStringAsync();
            }

        }
    }
}
