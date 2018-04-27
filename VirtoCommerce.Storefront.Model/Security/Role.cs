using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class Role : Entity
    {
        public string Name { get; set; }
        public IList<string> Permissions { get; set; } = new List<string>();
    }
}
