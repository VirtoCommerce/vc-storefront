using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Catalog
{
    public partial class ProductAssociation : Association
    {        
        public string ProductId { get; set; }
        public Product Product { get; set; }     
    }
}
