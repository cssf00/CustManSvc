using System;
using System.ComponentModel.DataAnnotations;

namespace CustManSvc.API.Service.Database
{
    // Customer Database object
    public class Customer
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
