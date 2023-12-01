//using System.ComponentModel;
using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Cosmos;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Linq;

namespace AZ_WebApp1.cosmodb
{
    public class CosmoDBService : ICosmoDBService
    {
        private ILogger _logger;
        private IConfigurationRoot _configRoot;
        private IConfigurationSection _scTradeTrackerSection;

        //// The Azure Cosmos DB endpoint for running this sample.
        //private static readonly string EndpointUri = System.Configuration.ConfigurationManager.AppSettings["EndPointUri"];
        private string? _endpointUri;

        //// The primary key for the Azure Cosmos account.
        //private static readonly string PrimaryKey = System.Configuration.ConfigurationManager.AppSettings["PrimaryKey"];
        private string? _primaryKey;

        // The Cosmos client instance
        private CosmosClient _cosmosClient;

        // The database we will create
        private Database _database;

        // The container we will create.
        private Microsoft.Azure.Cosmos.Container _container;

        // The name of the database and container we will create
        private string? _databaseId;
        private string? _containerId;

        public CosmoDBService(IConfiguration configRoot)
        {
            _configRoot = (IConfigurationRoot)configRoot;

            SetDBConfigurations();

            CreateDatabaseAsync().GetAwaiter().GetResult();
        }

        private void SetDBConfigurations()
        {
            _scTradeTrackerSection = _configRoot.GetSection("cosmodb");
            _endpointUri = _scTradeTrackerSection.GetValue<string>("EndpointUri");
            _primaryKey = _scTradeTrackerSection.GetValue<string>("PrimaryKey");
            _databaseId = _scTradeTrackerSection.GetValue<string>("DatabaseName");
            _containerId = _scTradeTrackerSection.GetValue<string>("ContainerName");
        }

        // <CreateDatabaseAsync>w
        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            this._cosmosClient = new CosmosClient(_endpointUri, _primaryKey, new CosmosClientOptions() { ApplicationName = "AZ_WebApp1" });

            // Get or Create a new database
            this._database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId);
            this._container = this._database.GetContainer(_containerId);
        }

        // <ScaleContainerAsync>
        /// <summary>
        /// Scale the throughput provisioned on an existing Container.
        /// You can scale the throughput (RU/s) of your container up and down to meet the needs of the workload. Learn more: https://aka.ms/cosmos-request-units
        /// </summary>
        /// <returns></returns>
        private async Task ScaleContainerAsync()
        {
            // Read the current throughput
            int? throughput = await this._container.ReadThroughputAsync();
            if (throughput.HasValue)
            {
                Console.WriteLine("Current provisioned throughput : {0}\n", throughput.Value);
                int newThroughput = throughput.Value + 100;
                // Update throughput
                //await this._container.ReplaceThroughputAsync(newThroughput);
                //Console.WriteLine("New provisioned throughput : {0}\n", newThroughput);
            }

        }

        public async Task AddItemToContainer<ItemType>(ItemType item, string itemId, string partitionKey) where ItemType : class
        {
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<ItemType> readResponse = await this._container.ReadItemAsync<ItemType>(itemId, new PartitionKey("id"));
                Console.WriteLine("Item in database with id: {0} already exists\n", itemId);
                //Console.WriteLine("Item in database with id: {0} already exists\n", itemResponse.Resource.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<ItemType> insertResponse = await this._container.CreateItemAsync<ItemType>(item, new PartitionKey("id"));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", itemId, insertResponse.RequestCharge);
            }
        }

        /// <summary>
        /// Takes a predefined parameterized query and runs it on the current DB.
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="parameterizedQuery"></param>
        /// <returns></returns>
        public async Task<List<ItemType>> ExecuteParameterizedQuery<ItemType>(QueryDefinition parameterizedQuery) where ItemType : class
        {
            List<ItemType> queryResult = new List<ItemType>();

            // Query multiple items from container
            using FeedIterator<ItemType> filteredFeed = this._container.GetItemQueryIterator<ItemType>(
                queryDefinition: parameterizedQuery
            );

            while( filteredFeed.HasMoreResults ) 
            {
                FeedResponse<ItemType> dataSet = await filteredFeed.ReadNextAsync();

                if (dataSet != null)
                {
                    foreach (ItemType item in dataSet)
                    {
                        if (item != null && !queryResult.Contains(item))
                        {
                            queryResult.Add(item);
                        }
                    }
                }
            }

            return queryResult;
        }

        public async Task UpdateItem<ItemType>(ItemType item) where ItemType : class
        {
            var x = await this._container.UpsertItemAsync<ItemType>(item);
        }

        public async Task<ItemType> ReadItemFromContainer<ItemType>(string itemId, string partitionKey) where ItemType : class
        {
            ItemResponse<ItemType> itemResponse = await this._container.ReadItemAsync<ItemType>(itemId, new PartitionKey("id"));
            var itemBody = itemResponse.Resource;

            return itemBody as ItemType;
        }

        // <ReplaceContainerItemAsync>
        /// <summary>
        /// Replace an item in the container
        /// </summary>
        public async Task ReplaceContainerItemAsync<ItemType>(ItemType newItem, string itemId, string partitionKey) where ItemType : class
        {
            ItemResponse<ItemType> itemResponse = await this._container.ReadItemAsync<ItemType>(itemId, new PartitionKey("id"));
            var itemBody = itemResponse.Resource;

            /// Values should be set elsewhere in the flow.

            //// replace the item with the updated content
            itemResponse = await this._container.ReplaceItemAsync<ItemType>(itemBody, itemId, new PartitionKey("id"));

            if (itemResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to replace the item with id {itemId} in contianer {_containerId}");
            }
        }

        /// <summary>
        /// Delete an item in the container
        /// </summary>
        private async Task DeleteContainerItemAsync<ItemType>(ItemType item, string itemId, string partitionKey)
        {
            //// Delete an item. Note we must provide the partition key value and id of the item to delete
            ItemResponse<ItemType> wakefieldFamilyResponse = await this._container.DeleteItemAsync<ItemType>(itemId, new PartitionKey("id"));
            //Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, familyId);
        }

        // <DeleteDatabaseAndCleanupAsync>
        /// <summary>
        /// Delete the database and dispose of the Cosmos Client instance
        /// </summary>
        private async Task DeleteDatabaseAndCleanupAsync()
        {
            DatabaseResponse databaseResourceResponse = await this._database.DeleteAsync();
            // Also valid: await this.cosmosClient.Databases["FamilyDatabase"].DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", this._databaseId);

            //Dispose of CosmosClient
            this._cosmosClient.Dispose();
        }
    }
}
