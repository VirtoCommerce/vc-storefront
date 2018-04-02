using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class Register 
    {
        [FromForm(Name = "customer[type]")]
        public string Type { get; set; }

        [FromForm(Name = "customer[first_name]")]
        public string FirstName { get; set; }
        [FromForm(Name = "customer[last_name]")]
        public string LastName { get; set; }
        [FromForm(Name = "customer[email]")]
        public string Email { get; set; }
        [FromForm(Name = "customer[user_name]")]
        public string UserName { get; set; }
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }
        [FromForm(Name = "customer[store_id]")]
        public string StoreId { get; set; }
        [FromForm(Name = "customer[role]")]
        public string Role { get; set; }
        [FromForm(Name = "customer[organization_name]")]
        public string OrganizationName { get; set; }
        [FromForm(Name = "customer[organization_id]")]
        public string OrganizationId { get; set; }
        [FromForm(Name = "customer[name]")]
        public string Name { get; set; }
        [FromForm(Name = "customer[address]")]
        public Address Address { get; set; }
    }
}
