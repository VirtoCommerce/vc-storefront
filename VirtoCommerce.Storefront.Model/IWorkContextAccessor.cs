using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model
{
    public interface IWorkContextAccessor
    {
        WorkContext WorkContext { get; set; }
    }
}
