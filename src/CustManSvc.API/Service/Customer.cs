using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustManSvc.API.Service
{
    // Customer Database object
    public class Customer
    {
        // [Key]
        public string Id { get; set; }
        
        // [JsonProperty(PropertyName = "firstName")]
        // [Required]
        public string FirstName { get; set; }

        // [JsonProperty(PropertyName = "lastName")]
        // [Required] // how to enforce required?
        public string LastName { get; set; }


        // [JsonProperty(PropertyName = "dateOfBirth")]
        // [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
