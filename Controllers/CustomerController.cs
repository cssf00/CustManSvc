using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustManSvc.Common;
using CustManSvc.DataTransferObject;
using CustManSvc.Service.Database;

namespace CustManSvc.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly IDatabaseClient _dbClient;

        public CustomerController(ILogger<CustomerController> logger, 
                IDatabaseClient custDBClient)
        {
            _logger = logger;
            _dbClient = custDBClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAll()
        {
            var dbCusts = await _dbClient.GetAllCustomersAsync();
            var dtoCusts = dbCusts.Select(c => ConvertToDTO(c)).ToList();
            return dtoCusts;
        }

        [HttpGet("{custID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDTO>> GetByID(int custID)
        {
            Customer dbCust = await _dbClient.GetCustomerByIDAsync(custID);
            if (dbCust == null) 
            {
                return NotFound(new ServiceError($"Customer with id {custID} not found"));
            }

            return ConvertToDTO(dbCust);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDTO>> Create(CustomerDTO reqCust)
        {
            if (reqCust.ID > 0)
            {
                return BadRequest(new ServiceError($"Customer ID must be zero on create: {reqCust.ID}"));
            }

            Customer dbCust = ConvertToDB(reqCust);
            await _dbClient.CreateCustomerAsync(dbCust);

            // Convert saved db cust back to data transfer object to respond back
            CustomerDTO respCust = ConvertToDTO(dbCust);
            return CreatedAtAction(nameof(GetByID), new {custID = respCust.ID}, respCust);
        }

        [HttpPut("{custID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(int custID, CustomerDTO reqCust)
        {
            if (custID == 0 || reqCust.ID == 0)
            {
                return BadRequest(new ServiceError($"Customer ID on query string and request body cannot be zero on update"));
            }

            if (custID != reqCust.ID)
            {
                return BadRequest(new ServiceError($"Customer ID on query string is different from request body"));
            }

            Customer dbCust = ConvertToDB(reqCust);
            try {
                await _dbClient.UpdateCustomerAsync(dbCust);
            }
            catch (RecordNotFoundException) {
                return NotFound(new ServiceError($"Customer with id {reqCust.ID} not found"));
            }
            
            return Ok();
        }

        [HttpDelete("{custID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDTO>> Delete(int custID)
        {
            Customer cust;
            try {
                cust = await _dbClient.DeleteAsync(custID);
            }
            catch (RecordNotFoundException) {
                return NotFound(new ServiceError($"Customer with id {custID} not found"));
            }

            // Convert db customer to dto before returning
            return Ok(ConvertToDTO(cust));
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> SearchName(string name)
        {
            _logger.LogDebug($"Searching customer names containing {name}");
            var dbCusts = await _dbClient.SearchNameAsync(name);
            return dbCusts.Select(c => ConvertToDTO(c)).ToList();
        }

        // Convert Customer db object to DTO
        private CustomerDTO ConvertToDTO(Customer dbCust)
        {
            CustomerDTO dtoCust = new CustomerDTO() {
                ID = dbCust.ID,
                FirstName = dbCust.FirstName,
                LastName = dbCust.LastName,
                DateOfBirth = dbCust.DateOfBirth.ToString(Constants.DateFormatRFC3339)
            };
            return dtoCust;
        }

        // Convert Customer DTO to db object
        private Customer ConvertToDB(CustomerDTO dtoCust)
        {
            Customer dbCust = new Customer() {
                ID = dtoCust.ID,
                FirstName = dtoCust.FirstName,
                LastName = dtoCust.LastName,
                // If there are timezone, convert to UTC
                DateOfBirth = DateTime.ParseExact(dtoCust.DateOfBirth, 
                    Constants.DateFormatRFC3339, DateTimeFormatInfo.InvariantInfo).ToUniversalTime()
            };
            return dbCust;
        }
    }
}
