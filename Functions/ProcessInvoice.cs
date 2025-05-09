using Microsoft.Azure.WebJobs;
using Azure.Storage.Blobs;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using Newtonsoft.Json;
using System.Text;

namespace PayTrackr.Functions
{
    public static class ProcessInvoice
    {
        [FunctionName("ProcessInvoice")]
        public static async Task Run(
            [BlobTrigger("invoices/{name}", Connection = "AzureWebJobsStorage")] Stream blobStream,
            string name,
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function processed blob\n Name: {name}");

            try
            {
                // Get the Document Intelligence credentials
                string endpoint = Environment.GetEnvironmentVariable("DocumentIntelligenceEndpoint");
                string key = Environment.GetEnvironmentVariable("DocumentIntelligenceKey");

                if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
                {
                    log.LogError("Document Intelligence configuration is missing.");
                    return;
                }

                // Process the invoice using Document Intelligence
                var credential = new AzureKeyCredential(key);
                var client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", blobStream);
                var result = operation.Value;

                if (result.Documents.Count > 0)
                {
                    var invoice = result.Documents[0];
                    var extractedData = new Dictionary<string, object>();

                    // Extract common invoice fields
                    if (invoice.Fields.TryGetValue("InvoiceId", out var invoiceIdField) && invoiceIdField.Value != null)
                        extractedData["InvoiceId"] = invoiceIdField.Value.AsString();

                    if (invoice.Fields.TryGetValue("InvoiceDate", out var invoiceDateField) && invoiceDateField.Value != null)
                        extractedData["InvoiceDate"] = invoiceDateField.Value.AsDate();

                    if (invoice.Fields.TryGetValue("DueDate", out var dueDateField) && dueDateField.Value != null)
                        extractedData["DueDate"] = dueDateField.Value.AsDate();

                    if (invoice.Fields.TryGetValue("VendorName", out var vendorNameField) && vendorNameField.Value != null)
                        extractedData["VendorName"] = vendorNameField.Value.AsString();

                    if (invoice.Fields.TryGetValue("CustomerName", out var customerNameField) && customerNameField.Value != null)
                        extractedData["CustomerName"] = customerNameField.Value.AsString();

                    if (invoice.Fields.TryGetValue("TotalTax", out var totalTaxField) && totalTaxField.Value != null)
                        extractedData["TotalTax"] = totalTaxField.Value.AsDouble();

                    if (invoice.Fields.TryGetValue("InvoiceTotal", out var invoiceTotalField) && invoiceTotalField.Value != null)
                        extractedData["InvoiceTotal"] = invoiceTotalField.Value.AsDouble();

                    if (invoice.Fields.TryGetValue("BillingAddress", out var billingAddressField) && billingAddressField.Value != null)
                        extractedData["BillingAddress"] = billingAddressField.Value.AsString();

                    // Save the extracted data to blob storage
                    string jsonData = JsonConvert.SerializeObject(extractedData, Formatting.Indented);
                    
                    // Get the connection string from the application settings
                    string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                    string resultsContainerName = Environment.GetEnvironmentVariable("ResultsContainerName") ?? "invoice-results";

                    // Upload the results to Azure Blob Storage
                    var blobServiceClient = new BlobServiceClient(connectionString);
                    var containerClient = blobServiceClient.GetBlobContainerClient(resultsContainerName);
                    
                    // Create the container if it doesn't exist
                    await containerClient.CreateIfNotExistsAsync();
                    
                    // Use the same name but with .json extension
                    string resultBlobName = Path.ChangeExtension(name, "json");
                    var blobClient = containerClient.GetBlobClient(resultBlobName);
                    
                    using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonData)))
                    {
                        await blobClient.UploadAsync(memoryStream, true);
                    }

                    log.LogInformation($"Successfully processed invoice {name} and saved results as {resultBlobName}");
                }
                else
                {
                    log.LogWarning($"No invoice document found in {name}");
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Error processing invoice {name}: {ex.Message}");
                throw; // Re-throw to trigger the Azure Function retry policy
            }
        }
    }
}