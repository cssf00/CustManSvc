using System;
using Microsoft.EntityFrameworkCore;

namespace CustManSvc.API.Service.InMemoryDB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }
    }
}
