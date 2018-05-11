using DotLiquid;
using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class ResetPassword : Drop
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Token { get; set; }
    }
}
