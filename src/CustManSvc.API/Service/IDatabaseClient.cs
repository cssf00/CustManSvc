using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustManSvc.API.Service
{
    // Interface to consumer needing access to database capabilities
    public interface IDatabaseClient
    {
        Task CreateCustomerAsync(Customer cust);

        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        Task<Customer> GetCustomerByIDAsync(string custID);

        // Returns false if customer does not exist
        Task<bool> UpdateCustomerAsync(Customer cust);

        // Returns false if customer does not exist, else true
        Task<bool> DeleteAsync(string custID);

        // Search substring match of first or last name
        Task<IList<Customer>> SearchNameAsync(string searchString);
    }
}
