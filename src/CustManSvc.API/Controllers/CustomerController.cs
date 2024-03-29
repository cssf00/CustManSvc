﻿using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustManSvc.API.Common;
using CustManSvc.API.DataTransferObject;
using CustManSvc.API.Service;
using CustManSvc.API.Service.InMemoryDB;
using CustManSvc.API.Filters;
using AutoMapper;

namespace CustManSvc.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [ServiceException]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly IDatabaseClient _dbClient;
        private readonly IMapper _objMapper;

        public CustomerController(ILogger<CustomerController> logger, 
                IDatabaseClient custDBClient, IMapper objMapper)
        {
            _logger = logger;
            _dbClient = custDBClient;
            _objMapper = objMapper;
        }

        ///<summary>
        ///  Get all customers
        ///</summary>
        /// <response code="200">OK, success</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CustomerDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAll()
        {
            var dbCusts = await _dbClient.GetAllCustomersAsync();
            var dtoCusts = dbCusts.Select(c => _objMapper.Map<CustomerDTO>(c)).ToList();
            return dtoCusts;
        }

        ///<summary>
        ///  Get customers by ID
        ///</summary>
        /// <param name="custID">Customer ID</param>
        /// <response code="200">OK, customer found</response>
        /// <response code="404">Not Found</response> 
        [HttpGet("{custID:minlength(1)}")]
        [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDTO>> GetByID(string custID)
        {
            Customer dbCust = await _dbClient.GetCustomerByIDAsync(custID);
            if (dbCust == null) 
            {
                return NotFound(new ServiceError($"Customer with id {custID} not found"));
            }

            return _objMapper.Map<CustomerDTO>(dbCust);
        }

        ///<summary>
        ///  Adds a new customer
        ///</summary>
        /// <param name="customer">Customer object to add</param>
        /// <response code="201">OK, customer created</response>
        /// <response code="400">Bad Request</response> 
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDTO>> Create(CustomerDTO customer)
        {
            if (!String.IsNullOrEmpty(customer.ID))
            {
                return BadRequest(new ServiceError($"Customer ID must be zero on create: {customer.ID}"));
            }

            Customer dbCust = _objMapper.Map<Customer>(customer);
            await _dbClient.CreateCustomerAsync(dbCust);

            // Convert saved db cust back to data transfer object to respond back
            CustomerDTO respCust = _objMapper.Map<CustomerDTO>(dbCust);

            return CreatedAtAction(nameof(GetByID), new {custID = respCust.ID}, respCust);
        }

        ///<summary>
        ///  Updates existing customer
        ///</summary>
        /// <param name="custID">Customer ID to update, cannot be zero</param>
        /// <param name="customer">Customer object to update. Customer.ID cannot be zero</param>
        /// <response code="200">OK, update successful</response>
        /// <response code="404">Not Found</response> 
        /// <response code="400">Bad Request</response> 
        [HttpPut("{custID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(string custID, CustomerDTO customer)
        {
            if (String.IsNullOrEmpty(custID) || String.IsNullOrEmpty(customer.ID))
            {
                return BadRequest(new ServiceError($"Customer ID on query string and request body cannot be zero on update"));
            }

            if (custID != customer.ID)
            {
                return BadRequest(new ServiceError($"Customer ID on query string is different from request body"));
            }

            Customer dbCust = _objMapper.Map<Customer>(customer);

            bool custFound = await _dbClient.UpdateCustomerAsync(dbCust);
            if (!custFound)
            {
                return NotFound(new ServiceError($"Customer with id {customer.ID} not found"));
            }
            
            return Ok();
        }

        ///<summary>
        ///  Deletes a customer
        ///</summary>
        /// <param name="custID">Customer ID to delete</param>
        /// <response code="200">OK, delete successful</response>
        /// <response code="404">Not Found</response> 
        [HttpDelete("{custID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string custID)
        {
            bool custFound = await _dbClient.DeleteAsync(custID);
            if (!custFound)
            {
                return NotFound(new ServiceError($"Customer with id {custID} not found"));
            }

            // Convert db customer to dto before returning
            return Ok();
        }

        ///<summary>
        ///  Substring search of customers' first and last names
        ///</summary>
        /// <param name="name">Name of the customer to search</param>
        /// <response code="200">OK, search successful</response>
        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> SearchName([FromQuery]string name)
        {
            _logger.LogDebug($"Searching customer names containing {name}");
            var dbCusts = await _dbClient.SearchNameAsync(name);
            return dbCusts.Select(c => _objMapper.Map<CustomerDTO>(c)).ToList();
        }
    }
}
