using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers
{
    public class VendorController : StorefrontControllerBase
    {
        private readonly IMemberService _customerService;
        private readonly ICatalogService _catalogService;

        public VendorController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IMemberService customerService, ICatalogService catalogService)
            : base(workContextAccessor, urlBuilder)
        {
            _customerService = customerService;
            _catalogService = catalogService;
        }

        /// <summary>
        /// GET: /vendor/{vendorId}
        /// This action is used by storefront to get vendor details by vendor ID
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> VendorDetails(string vendorId)
        {
            var vendor = (await _customerService.GetVendorsByIdsAsync(base.WorkContext.CurrentStore, base.WorkContext.CurrentLanguage, vendorId)).FirstOrDefault();

            if (vendor != null)
            {
                vendor.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    var criteria = new ProductSearchCriteria
                    {
                        VendorId = vendorId,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SortBy = SortInfo.ToString(sortInfos),
                        ResponseGroup = base.WorkContext.CurrentProductSearchCriteria.ResponseGroup
                    };
                    if (@params != null)
                    {
                        criteria.CopyFrom(@params);
                    }
                    var searchResult = _catalogService.SearchProducts(criteria);
                    return searchResult.Products;
                }, 1, ProductSearchCriteria.DefaultPageSize);

                WorkContext.CurrentPageSeo = vendor.SeoInfo;
                WorkContext.CurrentVendor = vendor;

                return View("vendor", WorkContext);
            }

            return NotFound();
        }
    }
}
