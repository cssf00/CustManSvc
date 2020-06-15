using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CustManSvc.Service.Database
{
    // Provide database capabilities, implements IDatabaseClient
    public class DatabaseClient : IDatabaseClient
    {
        private DatabaseContext _dbContext;

        public DatabaseClient(DatabaseContext dbContext)
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

        public async Task<Customer> GetCustomerByIDAsync(int custID)
        {
            Customer cust = await _dbContext.Customers.FindAsync(custID);
            return cust;
        }

        public async Task UpdateCustomerAsync(Customer cust)
        {
            try {
                _dbContext.Customers.Update(cust);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                // Concurrency error occurs when the customer we are trying to update no longer exists in db
                throw new RecordNotFoundException($"Customer record not found");
            }
        }

        public async Task<Customer> DeleteAsync(int custID)
        {
            Customer cust = await _dbContext.Customers.FindAsync(custID);
            if (cust == null) {
                throw new RecordNotFoundException($"Customer record not found");
            }

            _dbContext.Customers.Remove(cust);
            await _dbContext.SaveChangesAsync();
            return cust;
        }

        public async Task<IEnumerable<Customer>> SearchNameAsync(string searchString)
        {
            List<Customer> custs = await _dbContext.Customers
                .Where(c => (EF.Functions.Like(c.FirstName, $"%{searchString}%") 
                            || EF.Functions.Like(c.LastName, $"%{searchString}%")))
                .ToListAsync();
            
            return custs;
        }
    }
}
