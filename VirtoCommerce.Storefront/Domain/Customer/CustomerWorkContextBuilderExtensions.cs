using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CustomerWorkContextBuilderExtensions
    {
        public static Task WithVendorsAsync(this IWorkContextBuilder builder, Func<IMutablePagedList<Vendor>> factory)
        {
            builder.WorkContext.Vendors = factory();
            return Task.CompletedTask;
        }

        public static Task WithVendorsAsync(this IWorkContextBuilder builder, Store store, Language language)
        {
            var serviceProvider = builder.HttpContext.RequestServices;
            var customerService = serviceProvider.GetRequiredService<IMemberService>();
            var catalogService = serviceProvider.GetRequiredService<ICatalogService>();

            Func<int, int, IEnumerable<SortInfo>, IPagedList<Vendor>> factory = (pageNumber, pageSize, sortInfos) =>
            {
                var vendors = customerService.SearchVendors(store, language, null, pageNumber, pageSize, sortInfos);
                foreach (var vendor in vendors)
                {
                    vendor.Products = new MutablePagedList<Product>((pageNumber2, pageSize2, sortInfos2) =>
                    {
                        var vendorProductsSearchCriteria = new ProductSearchCriteria
                        {
                            VendorId = vendor.Id,
                            PageNumber = pageNumber2,
                            PageSize = pageSize2,
                            ResponseGroup = builder.WorkContext.CurrentProductSearchCriteria.ResponseGroup & ~ItemResponseGroup.ItemWithVendor,
                            SortBy = SortInfo.ToString(sortInfos2),
                        };
                        var searchResult = catalogService.SearchProducts(vendorProductsSearchCriteria);
                        return searchResult.Products;
                    }, 1, ProductSearchCriteria.DefaultPageSize);
                }
                return vendors;
            };
            return builder.WithVendorsAsync(() => new MutablePagedList<Vendor>(factory, 1, VendorSearchCriteria.DefaultPageSize));
        }

    }
}
