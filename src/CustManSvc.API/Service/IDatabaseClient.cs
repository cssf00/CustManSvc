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

        Task<Customer> GetCustomerByIDAsync(int custID);

        // Returns false if customer does not exist
        Task<bool> UpdateCustomerAsync(Customer cust);

        // Returns false if customer does not exist, returns a Customer deleted
        Task<(bool, Customer)> DeleteAsync(int custID);

        // Search substring match of first or last name
        Task<IEnumerable<Customer>> SearchNameAsync(string searchString);
    }
}
