using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using ml.Doki.Backend.Functions.Data;
using System.Linq;

namespace ml.Doki.Backend.Functions
{
    public static class ListDonations
    {
        [FunctionName("ListDonations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "list")] HttpRequest req,
            [Table("donations", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            // Parse the query parameters
            var from = req.Query.ContainsKey("from")
                ? DateTimeOffset.FromUnixTimeSeconds(int.Parse(req.Query["from"])).DateTime
                : DateTime.MinValue;
            var to = req.Query.ContainsKey("to")
                ? DateTimeOffset.FromUnixTimeSeconds(int.Parse(req.Query["to"])).DateTime
                : DateTime.Now;

            // Create the Azure Table Storage query
            var query = new TableQuery<DonationItem>().Copy();
            var donationList = await table.ExecuteQuerySegmentedAsync(query, null);
            var queryResult = donationList.Results
                .OrderByDescending(x => x.Timestamp)
                .Where(x => x.Timestamp.DateTime >= from && x.Timestamp.DateTime <= to)
                .ToList();

            log.LogInformation($"Successfully retrieved '{queryResult.Count}' donations.");
            return new OkObjectResult(queryResult);
        }
    }
}
