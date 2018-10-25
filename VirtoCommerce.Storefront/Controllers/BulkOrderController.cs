using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.BulkOrder;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute("bulkorder")]
    public class BulkOrderController : StorefrontControllerBase
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;

        public BulkOrderController(IWorkContextAccessor workContext, IStorefrontUrlBuilder urlBuilder, ICartBuilder cartBuilder, ICatalogService catalogSearchService)
            : base(workContext, urlBuilder)
        {
            _cartBuilder = cartBuilder;
            _catalogService = catalogSearchService;
        }

        // GET: /bulkorder
        [HttpGet]
        public ActionResult Index()
        {
            return View("bulk-order", WorkContext);
        }

        // POST: /bulkorder/addfielditems
        [HttpPost("addfielditems")]
        public async Task<ActionResult> AddFieldItems(BulkOrderItem[] items)
        {
            await EnsureThatCartExistsAsync();

            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                items = items.Where(i => !string.IsNullOrEmpty(i.Sku)).ToArray();
                if (items.Length == 0)
                {
                    return StoreFrontRedirect("~/bulkorder");
                }

                var notFoundedSkus = await TryAddItemsToCartAsync(items);
                foreach (var notFoundedSku in notFoundedSkus)
                {
                    ModelState.AddModelError(notFoundedSku, notFoundedSku);
                }

                if (ModelState.IsValid)
                {
                    return StoreFrontRedirect("~/cart");
                }
                else
                {
                    return View("bulk-order", WorkContext);
                }
            }
        }

        // POST: /bulkorder/addcsvitems
        [HttpPost("addcsvitems")]
        public async Task<ActionResult> AddCsvItems(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                return StoreFrontRedirect("~/bulkorder");
            }

            await EnsureThatCartExistsAsync();

            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var items = csv.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                               .Select(csvRecord => GetBulkOrderItemFromCsvRecord(csvRecord)).ToArray();
                if (items.Length == 0)
                {
                    return StoreFrontRedirect("~/bulkorder");
                }

                var notFoundedSkus = await TryAddItemsToCartAsync(items);
                foreach (var notFoundedSku in notFoundedSkus)
                {
                    ModelState.AddModelError(notFoundedSku, notFoundedSku);
                }

                if (ModelState.IsValid)
                {
                    return StoreFrontRedirect("~/cart");
                }
                else
                {
                    return View("bulk-order", WorkContext);
                }
            }
        }

        private async Task EnsureThatCartExistsAsync()
        {
            if (WorkContext.CurrentCart == null)
            {
                throw new StorefrontException("Cart not found");
            }

            await _cartBuilder.TakeCartAsync(WorkContext.CurrentCart.Value);
        }


        private async Task<string[]> TryAddItemsToCartAsync(BulkOrderItem[] bulkOrderItems)
        {
            var skus = bulkOrderItems.Select(i => i.Sku);
            //TODO: Need to replace from indexed search to special method GetProductByCodes when it will be presents in the catalog API
            var productSearchResult = await _catalogService.SearchProductsAsync(new ProductSearchCriteria
            {
                PageSize = skus.Count(),
                ResponseGroup = ItemResponseGroup.Variations | ItemResponseGroup.ItemWithPrices | ItemResponseGroup.Inventory,
                Terms = new[] { new Term { Name = "code", Value = string.Join(",", skus) } }
            });
            //Because product stores in index all codes of it variations and the catalog  search returns only main product we need to this concat with variations
            var foundProductsWithVariations = productSearchResult.Products.Concat(productSearchResult.Products.SelectMany(x => x.Variations));
            var addedSkus = new List<string>();
            foreach (var bulkOrderItem in bulkOrderItems)
            {
                var product = foundProductsWithVariations.FirstOrDefault(x => x.Sku.EqualsInvariant(bulkOrderItem.Sku));
                if (product != null)
                {
                    if (await _cartBuilder.AddItemAsync(product, bulkOrderItem.Quantity))
                    {
                        addedSkus.Add(product.Sku);
                    }
                }
            }
            await _cartBuilder.SaveAsync();

            return skus.Except(addedSkus).ToArray();
        }

        private BulkOrderItem GetBulkOrderItemFromCsvRecord(string csvRecord)
        {
            BulkOrderItem bulkOrderItem = null;

            var splitted = csvRecord.Split(',', ';', ' ', '\t');
            if (splitted.Length == 2)
            {
                int quantity = 0;
                int.TryParse(splitted[1], out quantity);
                if (quantity > 0)
                {
                    bulkOrderItem = new BulkOrderItem
                    {
                        Quantity = quantity,
                        Sku = splitted[0]
                    };
                }
            }

            return bulkOrderItem;
        }
    }
}
