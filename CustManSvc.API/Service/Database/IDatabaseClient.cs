using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustManSvc.API.Service.Database
{
    // Interface to consumer needing access to database capabilities
    public interface IDatabaseClient
    {
        Task CreateCustomerAsync(Customer cust);

        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        Task<Customer> GetCustomerByIDAsync(int custID);

        // Throws RecordNotFoundException when customer does not exist
        Task UpdateCustomerAsync(Customer cust);

        // Throws RecordNotFoundException when customer does not exist
        Task<Customer> DeleteAsync(int custID);

        // Search substring match of first or last name
        Task<IEnumerable<Customer>> SearchNameAsync(string searchString);
    }
}
