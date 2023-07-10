using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;

namespace CustManSvc.API.Service.CosmosDB
{
    public class CosmosDBClient : IDatabaseClient
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CosmosDBClient(IConfiguration config, CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(config["CustDB:Name"], config["CustDB:CustomerContainer"]);
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task CreateCustomerAsync(Customer cust)
        {
            cust.Id = Guid.NewGuid().ToString("N");

            using MemoryStream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync<Customer>(stream, cust, _jsonSerializerOptions);
            ResponseMessage response = await _container.CreateItemStreamAsync(stream, new PartitionKey(cust.Id));
            if (!response.IsSuccessStatusCode)
            {
                string errorMsg = $"Error creating new customer. StatusCode {response.StatusCode}, ErrorMessage {response.ErrorMessage}";
                throw new Exception(errorMsg);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var results = new List<Customer>();

            using FeedIterator<Customer> fit = _container.GetItemQueryIterator<Customer>(
                queryText: null,
                continuationToken: null,
                requestOptions: null
            );
            {
                while (fit.HasMoreResults)
                {
                    FeedResponse<Customer> resp = await fit.ReadNextAsync();
                    results.AddRange(resp.Resource);
                }
            }

            return results;
        }

        public async Task<Customer> GetCustomerByIDAsync(string custID)
        {
            ResponseMessage resp = await _container.ReadItemStreamAsync(custID, new PartitionKey(custID));
            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                string errorMsg = $"Error getting a customer. StatusCode {resp.StatusCode}, ErrorMessage {resp.ErrorMessage}";
                throw new Exception(errorMsg);
            }

            var cust = await JsonSerializer.DeserializeAsync<Customer>(resp.Content, _jsonSerializerOptions);
            return cust;
        }

        // Returns false if customer does not exist
        public async Task<bool> UpdateCustomerAsync(Customer cust)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync<Customer>(stream, cust, _jsonSerializerOptions);
            ResponseMessage resp = await _container.ReplaceItemStreamAsync(stream, cust.Id, new PartitionKey(cust.Id));
            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return false;

                string errorMsg = $"Error updating customer {cust.Id}. StatusCode {resp.StatusCode}, ErrorMessage {resp.ErrorMessage}";
                throw new Exception(errorMsg);
            }

            return true;
        }

        // Returns false if customer does not exist, returns a Customer deleted
        public async Task<bool> DeleteAsync(string custID)
        {
            ResponseMessage resp = await _container.DeleteItemStreamAsync(custID, new PartitionKey(custID));
            if (!resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == HttpStatusCode.NotFound)
                    return false;

                string errorMsg = $"Error deleting customer {custID}. StatusCode {resp.StatusCode}, ErrorMessage {resp.ErrorMessage}";
                throw new Exception(errorMsg);
            }

            return true;
        }

        // Search substring match of first or last name
        public async Task<IList<Customer>> SearchNameAsync(string searchString)
        {
            var result = new List<Customer>();

            using FeedIterator<Customer> custsIterator = _container.GetItemQueryIterator<Customer>(
                new QueryDefinition("select * from c where upper(c.firstName) = @searchString or upper(c.lastName) = @searchString").WithParameter("@searchString", searchString.ToUpperInvariant())
            );

            while (custsIterator.HasMoreResults)
            {
                FeedResponse<Customer> resp = await custsIterator.ReadNextAsync();
                result.AddRange(resp.Resource);
            }

            return result;
        }
    }
}