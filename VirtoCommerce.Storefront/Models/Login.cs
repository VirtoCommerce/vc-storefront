using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
   
    public partial class Login 
    {
        
        public string Email { get; set; }

        public bool UsernamesEnabled { get; set; }
        [FromForm(Name = "customer[user_name]")]
        public string Username { get; set; }
        [FromForm(Name = "customer[password]")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}
