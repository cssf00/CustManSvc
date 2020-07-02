using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using CustManSvc.API.DataTransferObject;

namespace CustManSvc.API.Tests.IntegrationTests
{
    public class CustomerControllerModifyTests : IClassFixture<AppFactoryForModifyTests<CustManSvc.API.Startup>>
    {
        private readonly AppFactoryForModifyTests<CustManSvc.API.Startup> _factory;

        public CustomerControllerModifyTests(AppFactoryForModifyTests<CustManSvc.API.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test_create_delete_customer()
        {
            var client = _factory.CreateClient();

            // Create new customer
            CustomerDTO dtoCust = new CustomerDTO {
                ID = 0,
                FirstName = "Peter",
                LastName = "Sampson",
                DateOfBirth = "1995-05-21T00:00:00Z"
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(dtoCust), Encoding.UTF8, "application/json");
            var createResponse = await client.PostAsync($"/api/customers", requestContent);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdCust = JsonSerializer.Deserialize<CustomerDTO>(await createResponse.Content.ReadAsStringAsync());
            Assert.Equal("Sampson", createdCust.LastName);

            // Delete the customer
            var deleteResponse = await client.DeleteAsync($"/api/customers/4");
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            var deletedCust = JsonSerializer.Deserialize<CustomerDTO>(await deleteResponse.Content.ReadAsStringAsync());
            Assert.Equal("Sampson", deletedCust.LastName);
        }

        [Fact]
        public async Task Test_update_customer()
        {
            var client = _factory.CreateClient();

            // Updates customer 1
            CustomerDTO dtoCust = new CustomerDTO {
                ID = 1,
                FirstName = "Samantha",
                LastName = "Jackson",
                DateOfBirth = "2000-03-04T00:00:00Z"
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(dtoCust), Encoding.UTF8, "application/json");
            var putResponse = await client.PutAsync($"/api/customers/1", requestContent);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            // Get customer 1 to check
            var getResponse = await client.GetAsync($"/api/customers/1");
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

            string getResponseContent = await getResponse.Content.ReadAsStringAsync();
            var updatedCust = JsonSerializer.Deserialize<CustomerDTO>(getResponseContent);
            Assert.Equal(1, updatedCust.ID);
            Assert.Equal("Samantha", updatedCust.FirstName);
            Assert.Equal("Jackson", updatedCust.LastName);
            Assert.Equal("2000-03-04T00:00:00Z", updatedCust.DateOfBirth);
        }
    }
}
