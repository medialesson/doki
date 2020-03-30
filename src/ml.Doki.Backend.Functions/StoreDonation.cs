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
using System.Net;
using System.Globalization;

namespace ml.Doki.Backend.Functions
{
    public static class StoreDonation
    {
        [FunctionName("StoreDonation")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "commit")] HttpRequest req,
            [Table("donations", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            // Parse the body input to a transfer model
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var donation = JsonConvert.DeserializeObject<DonationItem>(body);

            // Create the Azure Table Storage insert
            var insertOperation = TableOperation.Insert(donation);
            var result = await table.ExecuteAsync(insertOperation);

            if (result.HttpStatusCode == (int)HttpStatusCode.BadRequest)
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);

            log.LogInformation($"Successfully committed donation with ID '{donation.RowKey}' of '{donation.Amount}'.");
            return new OkResult();
        }
    }
}
