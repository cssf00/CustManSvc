using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using CustManSvc.API.DataTransferObject;

namespace CustManSvc.API.Tests.IntegrationTests
{
    public class CustomerControllerReadOnlyTests : IClassFixture<AppFactoryForReadOnlyTests<CustManSvc.API.Startup>>
    {
        private readonly AppFactoryForReadOnlyTests<Startup> _factory;

        public CustomerControllerReadOnlyTests(AppFactoryForReadOnlyTests<CustManSvc.API.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Test_get_all()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/customers");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            var dtoCusts = JsonSerializer.Deserialize<IEnumerable<CustomerDTO>>(responseContent);
    
            Assert.Equal(3, dtoCusts.Count());
        }

        [Fact]
        public async Task Test_get_one_correct()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/customers/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            var dtoCust = JsonSerializer.Deserialize<CustomerDTO>(responseContent);
    
            Assert.Equal(1, dtoCust.ID);
            Assert.Equal("Sammy", dtoCust.FirstName);
            Assert.Equal("Hosea", dtoCust.LastName);
            Assert.Equal("2001-03-04T00:00:00Z", dtoCust.DateOfBirth);
        }

        [Fact]
        public async Task Test_search_customer_name()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/customers/search?name=nn");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            var dtoCusts = JsonSerializer.Deserialize<IEnumerable<CustomerDTO>>(responseContent);
    
            // returns Johnny and Jenny
            Assert.Equal(2, dtoCusts.Count());
        }
    }
}
