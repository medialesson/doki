using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ml.Doki.Backend.Functions
{
    public static class KeepWarm
    {
        [FunctionName("KeepWarm")]
        public static void Run([TimerTrigger("0 */10 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"KeepWarm trigger executed at: {DateTime.Now}");
        }
    }
}
