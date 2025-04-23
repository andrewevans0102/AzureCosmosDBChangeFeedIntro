# Azure Cosmos DB Change Feed Intro

This project is a companion to my blog post on Azure Cosmos DB Change Feed. It is just meant as a sample implementation and a way to show how the feature works. I also used [claude](https://claude.ai) to help build this implementation. Some of the comments and names in the project came from claude and others came from me.

If you want to get this to work locally, you'll need to create a `local.settings.json` file in the repo with the following values:

```json
{
    "IsEncrypted": false,
    "Values": {
      "AzureWebJobsStorage": "UseDevelopmentStorage=true",
      "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
      "CosmosDBConnection": "<YOUR_COSMOS_DB_CONNECTION_STRING>",
      "SourceDatabaseName": "<SOURCE_CONTAINER>",
      "SourceCollectionName": "<SOURCE_COLLECTION_NAME>",
      "HistoryDatabaseName": "<HISTORY_CONTAINER>",
      "HistoryCollectionName": "<HISTORY_COLLECTION_NAME>"
    }
  }
```

If you wanted to deploy the function, you'll obviously have to add environment values with the same names etc. I only ran this locally when doing some research and playing with the functionality.

You'll also obviously need an Azure Cosmos DB collection setup for it to connect correctly etc. If you want to learn more about Azure Cosmos DB Change Feed, I recommend [checking out their documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/change-feed?context=%2Fazure%2Fcosmos-db%2Fnosql%2Fcontext%2Fcontext).