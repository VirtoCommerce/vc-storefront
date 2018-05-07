using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistration 
    {
        [FromForm(Name = "customer[photoUrl]")]
        public string PhotoUrl { get; set; }

        [FromForm(Name = "customer[first_name]")]
        public string FirstName { get; set; }

        [FromForm(Name = "customer[last_name]")]
        public string LastName { get; set; }

        [FromForm(Name = "customer[email]")]
        [EmailAddress]
        public string Email { get; set; }

        [FromForm(Name = "customer[user_name]")]
        [Required]
        public string UserName { get; set; }

        [FromForm(Name = "customer[password]")]
        [Required]
        public string Password { get; set; }

        [FromForm(Name = "customer[store_id]")]
        public string StoreId { get; set; }

        [FromForm(Name = "customer[name]")]
        public string Name { get; set; }

        [FromForm(Name = "customer[address]")]
        public Address Address { get; set; }
        [FromForm(Name = "customer[salutation]")]
        public string Salutation { get; set; }
        [FromForm(Name = "customer[fullName]")]
        public string FullName { get; set; }
        [FromForm(Name = "customer[middleName]")]
        public string MiddleName { get; set; }
        [FromForm(Name = "customer[birthDate]")]
        public DateTime? BirthDate { get; set; }
        [FromForm(Name = "customer[timeZone]")]
        public string TimeZone { get; set; }     
      
    }
}
