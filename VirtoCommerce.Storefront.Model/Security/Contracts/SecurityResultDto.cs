using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Security.Contracts
{
    public class SecurityResultDto
    {
        public bool? Succeeded { get; set; }
        public IList<string> Errors { get; set; }
    }
}
