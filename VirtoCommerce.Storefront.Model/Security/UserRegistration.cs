using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistration 
    {
        [FromForm(Name = "customer[first_name]")]
        public string FirstName { get; set; }
        [FromForm(Name = "customer[last_name]")]
        public string LastName { get; set; }
        [FromForm(Name = "customer[email]")]
        [EmailAddress]
        public string Email { get; set; }
        [FromForm(Name = "customer[user_name]")]
        public string UserName { get; set; }
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }
        [FromForm(Name = "customer[store_id]")]
        public string StoreId { get; set; }
        [FromForm(Name = "customer[name]")]
        public string Name { get; set; }
        [FromForm(Name = "customer[address]")]
        public Address Address { get; set; }
    }
}
