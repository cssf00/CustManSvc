using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using CustManSvc.API.Service;

namespace CustManSvc.API.Service.CosmosDB
{
    public class CosmosDBClient : IDatabaseClient
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosDBClient(IConfiguration config, CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(config["CustDB:Name"], config["CustDB:CustomerContainer"]);
        }

        public async Task CreateCustomerAsync(Customer cust)
        {
            cust.ID = (await GetNextID()).ToString();
            ItemResponse<Customer> response = await _container.CreateItemAsync<Customer>(cust);
        }

        private async Task<int> GetNextID()
        {
            QueryDefinition qd = new QueryDefinition("select max(convert(int, id)) from Customers");

            int maxID = 0;
            using (FeedIterator<int> feedIterator = _container.GetItemQueryIterator<int>(qd))
            {
                while (feedIterator.HasMoreResults)
                {
                    maxID = (await feedIterator.ReadNextAsync()).SingleOrDefault();
                    break;
                }
            }

            return maxID + 1;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await Task.FromResult(new List<Customer>());
        }

        public async Task<Customer> GetCustomerByIDAsync(int custID)
        {
            return await Task.FromResult(new Customer());
        }

        // Returns false if customer does not exist
        public async Task<bool> UpdateCustomerAsync(Customer cust)
        {
            return await Task.FromResult(true);
        }

        // Returns false if customer does not exist, returns a Customer deleted
        public async Task<(bool, Customer)> DeleteAsync(int custID)
        {
            return await Task.FromResult((true, new Customer()));
        }

        // Search substring match of first or last name
        public async Task<IEnumerable<Customer>> SearchNameAsync(string searchString)
        {
            return await Task.FromResult(new List<Customer>());
        }
    }
}