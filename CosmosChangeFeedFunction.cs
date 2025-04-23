using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CosmosChangeFeedFunction
{
    public class ChangeStreamProcessor
    {
        private readonly ILogger<ChangeStreamProcessor> _logger;

        public ChangeStreamProcessor(ILogger<ChangeStreamProcessor> logger)
        {
            _logger = logger;
        }

        [Function("CosmosDBTrigger")]
        [CosmosDBOutput(
            databaseName: "%HistoryDatabaseName%",
            containerName: "%HistoryCollectionName%",
            Connection = "CosmosDBConnection")]
        public List<dynamic> Run(
            [CosmosDBTrigger(
                databaseName: "%SourceDatabaseName%",
                containerName: "%SourceCollectionName%",
                Connection = "CosmosDBConnection",
                LeaseContainerName = "leases",
                CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<JsonElement> documents)
        {
            var outputDocuments = new List<dynamic>();
            
            if (documents != null && documents.Count > 0)
            {
                _logger.LogInformation($"Processing {documents.Count} document(s) from change feed");
                
                foreach (var document in documents)
                {
                    try
                    {
                        // Extract the id from the JsonElement
                        string documentId = "";
                        if (document.TryGetProperty("id", out var idElement) && idElement.ValueKind != JsonValueKind.Null)
                        {
                            documentId = idElement.GetString() ?? Guid.NewGuid().ToString();
                        }
                        else
                        {
                            documentId = Guid.NewGuid().ToString();
                            _logger.LogWarning("Document didn't contain an id property, generated a new one");
                        }
                        
                        // Detect operation type
                        string operationType = "";
                        if (document.TryGetProperty("operationType", out var oTypeElement) && oTypeElement.ValueKind != JsonValueKind.Null)
                        {
                            operationType = oTypeElement.GetString() ?? "unknown";
                        }
                        
                        // Create history document
                        var historyDocument = new
                        {
                            id = Guid.NewGuid().ToString(),
                            sourceId = documentId,
                            sourceDocumentData = document.ToString(),
                            changeTimestamp = DateTime.UtcNow,
                            operationType = operationType
                        };

                        // Add to output collection
                        outputDocuments.Add(historyDocument);
                        
                        _logger.LogInformation($"Added history record for document with ID: {documentId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing document: {ex.Message}");
                    }
                }
            }
            
            return outputDocuments;
        }
    }
}