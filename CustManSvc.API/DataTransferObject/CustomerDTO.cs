using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using CustManSvc.API.Common;

namespace CustManSvc.API.DataTransferObject
{
    // Customer data transfer object, that is sent to or received from clients of the service
    public class CustomerDTO
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        
        [JsonPropertyName("firstName")]
        [Required]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [Required]
        public string LastName { get; set; }

        [JsonPropertyName("dateOfBirth")]
        [Required]
        [DateFormatValidation(Constants.DateFormatRFC3339)]
        public string DateOfBirth { get; set; }
    }
}
