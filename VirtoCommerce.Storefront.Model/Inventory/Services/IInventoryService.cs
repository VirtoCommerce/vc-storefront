using System.Collections.Generic;
using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Inventory.Services
{
    public interface IInventoryService
    {
        Task EvaluateProductInventoriesAsync(IEnumerable<Product> products, WorkContext workContext);
        Task<FulfillmentCenter> GetFulfillmentCenterByIdAsync(string id);
        FulfillmentCenter GetFulfillmentCenterById(string id);
        IPagedList<FulfillmentCenter> SearchFulfillmentCenters(FulfillmentCenterSearchCriteria criteria);
        Task<IPagedList<FulfillmentCenter>> SearchFulfillmentCentersAsync(FulfillmentCenterSearchCriteria criteria);

    }
}
