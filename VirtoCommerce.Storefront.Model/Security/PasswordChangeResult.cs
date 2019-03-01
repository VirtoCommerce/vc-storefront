using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class PasswordChangeResult
    {
        public bool? Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
