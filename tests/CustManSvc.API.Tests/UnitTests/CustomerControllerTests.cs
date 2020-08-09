using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustManSvc.API;
using CustManSvc.API.Controllers;
using CustManSvc.API.DataTransferObject;
using CustManSvc.API.Service.Database;
using Xunit;
using Moq;
using AutoMapper;

namespace CustManSvc.API.Tests.UnitTests
{
	public class CustomerControllerTests
	{
		private readonly IMapper _objMapper;

		public CustomerControllerTests()
		{
			_objMapper = new MapperConfiguration(c => c.AddProfile(new ObjectMappingProfile())).CreateMapper();
		}

		[Fact]
		public async void UpdateMissingCustomerReturnsNotFound()
		{
			var mockLogger = new Mock<ILogger<CustomerController>>();

			var mockDBClient = new Mock<IDatabaseClient>();
			mockDBClient.Setup(e => e.UpdateCustomerAsync(It.Is<Customer>(c => c.ID == 1)))
				.ReturnsAsync(false);

			var customerController = new CustomerController(mockLogger.Object, mockDBClient.Object, _objMapper);

			ActionResult result = await customerController.Update(1, 
				new CustomerDTO(){ID = 1, DateOfBirth = @"2019-02-01T00:00:00Z"});
			Assert.True(result.GetType() == typeof(NotFoundObjectResult));
			var notFound = (NotFoundObjectResult)result;
			Assert.Equal(404, notFound.StatusCode);
		}

		[Fact]
		public async void DeleteMissingCustomerReturnsNotFound()
		{
			var mockLogger = new Mock<ILogger<CustomerController>>();

			var mockDBClient = new Mock<IDatabaseClient>();
			mockDBClient.Setup(e => e.DeleteAsync(It.Is<int>(i => i == 1)))
				.ReturnsAsync((false, 
					new Customer{ID = 1, DateOfBirth = new DateTime(2019, 2, 1, 0,0,0, DateTimeKind.Utc)}));

			var customerController = new CustomerController(mockLogger.Object, mockDBClient.Object, _objMapper);

			IActionResult result = await customerController.Delete(1);
			Assert.True(result.GetType() == typeof(NotFoundObjectResult));
			var notFound = (NotFoundObjectResult)result;
			Assert.Equal(404, notFound.StatusCode);
		}

	}
}