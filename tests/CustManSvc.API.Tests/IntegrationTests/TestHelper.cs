using System;
using System.Collections.Generic;
using CustManSvc.API.Service.Database;
using CustManSvc.API.Service.Database.InMemoryDB;

namespace CustManSvc.API.Tests.IntegrationTests
{
    public static class TestHelper
    {
        public static void PopulateTestData(DatabaseContext dbc)
        {
            dbc.AddRange(GetCustomerTestData());
            dbc.SaveChanges();
        }

        public static List<Customer> GetCustomerTestData()
        {
            return new List<Customer>()
            {
                new Customer() { ID = 1, FirstName = "Sammy", LastName = "Hosea", DateOfBirth = new DateTime(2001, 3, 4, 0, 0, 0, DateTimeKind.Utc) },
                new Customer() { ID = 2, FirstName = "Jenny", LastName = "Warford", DateOfBirth = new DateTime(2002, 5, 9, 0, 0, 0, DateTimeKind.Utc) },
                new Customer() { ID = 3, FirstName = "Johnny", LastName = "Linden", DateOfBirth = new DateTime(1999, 10, 24, 0, 0, 0, DateTimeKind.Utc) }
            };
        }
    }
}