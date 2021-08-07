using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustManSvc.API.Service;

namespace CustManSvc.API.Service.InMemoryDB
{
    // Provide database capabilities, implements IDatabaseClient
    public class InMemoryDBClient : IDatabaseClient
    {
        private DatabaseContext _dbContext;

        public InMemoryDBClient(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateCustomerAsync(Customer cust)
        {
            _dbContext.Customers.Add(cust);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            List<Customer> custs = await _dbContext.Customers.ToListAsync();
            return custs;
        }

        public async Task<Customer> GetCustomerByIDAsync(string custID)
        {
            Customer cust = await _dbContext.Customers.FindAsync(custID);
            return cust;
        }

        public async Task<bool> UpdateCustomerAsync(Customer cust)
        {
            bool custFound = true;
            try {
                _dbContext.Customers.Update(cust);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                // Concurrency error occurs when the customer we are trying to update no longer exists in db
                custFound = false;
            }

            return custFound;
        }

        // bool = custFound
        public async Task<bool> DeleteAsync(string custID)
        {
            Customer cust = await _dbContext.Customers.FindAsync(custID);
            if (cust == null) {
                return false;
            }

            _dbContext.Customers.Remove(cust);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IList<Customer>> SearchNameAsync(string searchString)
        {
            List<Customer> custs = await _dbContext.Customers
                .Where(c => (EF.Functions.Like(c.FirstName, $"%{searchString}%") 
                            || EF.Functions.Like(c.LastName, $"%{searchString}%")))
                .ToListAsync();
            
            return custs;
        }
    }
}
