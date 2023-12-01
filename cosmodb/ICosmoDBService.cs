//using System.ComponentModel;
using Microsoft.Azure.Cosmos;

namespace AZ_WebApp1.cosmodb
{
    public interface ICosmoDBService
    {
        Task AddItemToContainer<ItemType>(ItemType item, string itemId, string partitionKey) where ItemType : class;
        Task<ItemType> ReadItemFromContainer<ItemType>(string itemId, string partitionKey) where ItemType : class;
        Task ReplaceContainerItemAsync<ItemType>(ItemType newItem, string itemId, string partitionKey) where ItemType : class;

        Task<List<ItemType>> ExecuteParameterizedQuery<ItemType>(QueryDefinition parameterizedQuery) where ItemType : class;
    }
}