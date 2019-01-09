using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class LoginProvider : ValueObject
    {
        public string AuthenticationType { get; set; }

        public string Caption { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}
