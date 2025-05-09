using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Azure.Storage.Blobs.Models;

namespace PayTrackr.Functions
{
    public static class GetInvoices
    {
        [FunctionName("GetInvoices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processing a request to get invoice data.");

            try
            {
                // Get the connection string from the application settings
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string resultsContainerName = Environment.GetEnvironmentVariable("ResultsContainerName") ?? "invoice-results";

                // Initialize blob service client
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(resultsContainerName);

                // Check if container exists
                if (!await containerClient.ExistsAsync())
                {
                    return new OkObjectResult(new List<object>());
                }

                // List all blobs in the container
                List<object> invoiceDataList = new List<object>();
                
                // Get all blobs in the container
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    // Get the blob client
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    
                    // Download the blob content
                    BlobDownloadInfo download = await blobClient.DownloadAsync();
                    
                    using (StreamReader reader = new StreamReader(download.Content))
                    {
                        // Read the content as string
                        string content = await reader.ReadToEndAsync();
                        
                        // Deserialize the JSON content
                        var invoiceData = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
                        
                        // Add the blob name and last modified date
                        invoiceData["BlobName"] = blobItem.Name;
                        invoiceData["LastModified"] = blobItem.Properties.LastModified;
                        
                        // Add to the list
                        invoiceDataList.Add(invoiceData);
                    }
                }

                // Sort by last modified date (newest first)
                var sortedList = invoiceDataList
                    .OrderByDescending(i => ((Dictionary<string, object>)i)["LastModified"])
                    .ToList();

                return new OkObjectResult(sortedList);
            }
            catch (Exception ex)
            {
                log.LogError($"Error retrieving invoice data: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}