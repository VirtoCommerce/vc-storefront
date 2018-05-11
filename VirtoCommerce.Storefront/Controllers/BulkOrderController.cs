using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.BulkOrder;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers
{
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
        [HttpPost]
        public async Task<ActionResult> AddFieldItems(BulkOrderItem[] items)
        {
            await EnsureThatCartExistsAsync();

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext.CurrentCart.Value)).LockAsync())
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
        [HttpPost]
        public async Task<ActionResult> AddCsvItems(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                return StoreFrontRedirect("~/bulkorder");
            }

            await EnsureThatCartExistsAsync();

            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext.CurrentCart.Value)).LockAsync())
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

        private static string GetAsyncLockCartKey(ShoppingCart cart)
        {
            return string.Join(":", "Cart", cart.Id, cart.Name, cart.CustomerId);
        }

        private async Task<string[]> TryAddItemsToCartAsync(BulkOrderItem[] bulkOrderItems)
        {
            var skus = bulkOrderItems.Select(i => i.Sku);
            var productSearchResult = await _catalogService.SearchProductsAsync(new ProductSearchCriteria
            {
                PageSize = skus.Count(),
                Terms = new[] { new Term { Name = "code", Value = string.Join(",", skus) } }
            });

            foreach (var product in productSearchResult.Products)
            {
                var bulkOrderItem = bulkOrderItems.FirstOrDefault(i => i.Sku == product.Sku);
                if (bulkOrderItem != null)
                {
                    await _cartBuilder.AddItemAsync(product, bulkOrderItem.Quantity);
                }
            }

            await _cartBuilder.SaveAsync();

            var notFoundedSkus = bulkOrderItems.Select(x => x.Sku).Except(productSearchResult.Products.Select(x => x.Sku))
                                               .Distinct().ToArray();
            return notFoundedSkus;
        }

        private BulkOrderItem GetBulkOrderItemFromCsvRecord(string csvRecord)
        {
            BulkOrderItem bulkOrderItem = null;

            var splitted = csvRecord.Split(',',';',' ','\t');
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