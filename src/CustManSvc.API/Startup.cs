using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using CustManSvc.API.Service;
using CustManSvc.API.Service.CosmosDB;
using NSwag.AspNetCore;
using AutoMapper;

namespace CustManSvc.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowCors", builder =>
                    {
                        builder.WithOrigins("http://localhost:3000", "https://localhost:3001")
                            .AllowCredentials()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton<CosmosClient>(p => 
                new CosmosClient(
                    accountEndpoint: Configuration["CustDB:URI"],
                    authKeyOrResourceToken: Configuration["CustDB:PrimaryKey"]
                )
            );
            services.AddScoped<IDatabaseClient, CosmosDBClient>();
            //services.AddDbContext<DatabaseContext>(opts => opts.UseInMemoryDatabase("CustomerDB"));
            //services.AddScoped<IDatabaseClient, InMemoryDBClient>();
            
            var mapperConfig = new MapperConfiguration(c => c.AddProfile(new ObjectMappingProfile()));
            services.AddSingleton<IMapper>(sp => mapperConfig.CreateMapper());

            // Swagger documentation
            services.AddSwaggerDocument(config => {
                config.PostProcess = doc =>
                {
                    doc.Info.Version = "v1";
                    doc.Info.Title = "CustManSvc.API";
                    doc.Info.Description = "Customer management service";
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Adds open api and swagger middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors("AllowCors");
            });
        }
    }
}
