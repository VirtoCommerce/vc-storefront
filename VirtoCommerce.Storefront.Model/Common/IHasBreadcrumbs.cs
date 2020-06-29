using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public interface IHasBreadcrumbs
    {
        IEnumerable<Breadcrumb> GetBreadcrumbs();
    }
}
