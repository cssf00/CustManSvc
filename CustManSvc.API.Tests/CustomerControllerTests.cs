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

namespace CustManSvc.API.Tests
{
    public class ItemControllerTests : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        private readonly InMemoryApplicationFactory<Startup> _factory;

        public ItemControllerTests(InMemoryApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test_create_customer()
        {
            var client = _factory.CreateClient();

            CustomerDTO dtoCust = new CustomerDTO {
                ID = 0,
                FirstName = "Peter",
                LastName = "Sampson",
                DateOfBirth = "1995-05-21T00:00:00Z"
            };
            var requestContent = new StringContent(JsonSerializer.Serialize(dtoCust), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/customers", requestContent);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            // Prepopulated 3 customers, so the return ID should be 4
            Assert.Equal("/api/customers/4", response.Headers.Location.AbsolutePath);

            var resultCust = JsonSerializer.Deserialize<CustomerDTO>(await response.Content.ReadAsStringAsync());
            Assert.Equal(4, resultCust.ID);
        }

        [Fact]
        public async Task Test_get_one_correct()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/customers/1");
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            var dtoCust = JsonSerializer.Deserialize<CustomerDTO>(responseContent);
    
            Assert.Equal(1, dtoCust.ID);
            Assert.Equal("Sammy", dtoCust.FirstName);
            Assert.Equal("Hosea", dtoCust.LastName);
            Assert.Equal("2001-03-04T00:00:00Z", dtoCust.DateOfBirth);
        }

        [Fact]
        public async Task Test_get_all()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/customers");
            response.EnsureSuccessStatusCode();

            string responseContent = await response.Content.ReadAsStringAsync();
            var dtoCusts = JsonSerializer.Deserialize<IEnumerable<CustomerDTO>>(responseContent);
    
            Assert.Equal(3, dtoCusts.Count());
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
