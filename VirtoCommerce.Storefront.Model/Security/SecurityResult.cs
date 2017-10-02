using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class SecurityResult
    {
        public bool Succeeded { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
    }
}
