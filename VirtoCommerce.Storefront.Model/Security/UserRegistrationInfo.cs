using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserRegistrationInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
