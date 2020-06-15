using System;
using System.ComponentModel.DataAnnotations;
using CustManSvc.API.Common;

namespace CustManSvc.API.DataTransferObject
{
    // Customer data transfer object, that is sent to or received from clients of the service
    public class CustomerDTO
    {
        public int ID { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DateFormatValidation(Constants.DateFormatRFC3339)]
        public string DateOfBirth { get; set; }
    }
}
