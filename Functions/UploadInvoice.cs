using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace PayTrackr.Functions
{
    public static class UploadInvoice
    {
        [FunctionName("UploadInvoice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processing a request to upload invoice.");

            try
            {
                // Check if the request contains form data
                if (!req.HasFormContentType)
                {
                    return new BadRequestObjectResult("Expected form data with a file.");
                }

                // Get the form data
                var form = await req.ReadFormAsync();
                var file = form.Files.FirstOrDefault();
                
                if (file == null)
                {
                    return new BadRequestObjectResult("No file found in the request.");
                }

                // Validate that the file is a PDF
                if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                    !Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    return new BadRequestObjectResult("Only PDF files are supported.");
                }

                // Get the connection string from the application settings
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                string containerName = Environment.GetEnvironmentVariable("InvoiceContainerName") ?? "invoices";

                // Create a unique blob name
                string blobName = $"{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid()}-{file.FileName}";

                // Upload the file to Azure Blob Storage
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                
                // Create the container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();
                
                var blobClient = containerClient.GetBlobClient(blobName);
                
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                log.LogInformation($"File {file.FileName} uploaded to blob storage as {blobName}");
                
                // Queue the file for processing by Document Intelligence
                // In a real application, you would add a message to a queue here
                // For now, we'll just return success
                
                var response = new
                {
                    fileName = file.FileName,
                    blobName = blobName,
                    uploadedAt = DateTime.UtcNow,
                    status = "uploaded",
                    message = "File uploaded successfully and queued for processing."
                };

                return new OkObjectResult(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                log.LogError($"Error uploading invoice: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}