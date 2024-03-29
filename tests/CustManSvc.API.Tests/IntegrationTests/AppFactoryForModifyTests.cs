using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CustManSvc.API.Service.Database;
using CustManSvc.API.Service.Database.InMemoryDB;

namespace CustManSvc.API.Tests.IntegrationTests
{
    public class AppFactoryForModifyTests<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Remove the app's DatabaseContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("EmptyDB");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DatabaseContext>();
                    db.Database.EnsureCreated();

                    var logger = scopedServices.GetRequiredService<ILogger<AppFactoryForReadOnlyTests<TStartup>>>();
                    try {
                        TestHelper.PopulateTestData(db);
                    }
                    catch (Exception ex) {
                        logger.LogError(ex, "Error occurs when populating test data: {0}", ex.Message);
                    }
                }
            });
        }
    }
}