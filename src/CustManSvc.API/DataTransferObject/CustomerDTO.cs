using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using CustManSvc.API.Common;

namespace CustManSvc.API.DataTransferObject
{
    // Customer data transfer object, that is sent to or received from clients of the service
    public class CustomerDTO
    {
        ///<summary>
        /// Customer ID
        ///</summary>
        [JsonPropertyName("id")]
        public string ID { get; set; }
        
        [JsonPropertyName("firstName")]
        [Required]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        [Required]
        public string LastName { get; set; }

        ///<summary>
        /// Customer's Date of Birth in UTC in RFC3339 format: "2019-12-23T00:00:00Z"
        ///</summary>
        [JsonPropertyName("dateOfBirth")]
        [Required]
        [RequiredDateFormat(Constants.DateFormatRFC3339)]
        public string DateOfBirth { get; set; }
    }
}
