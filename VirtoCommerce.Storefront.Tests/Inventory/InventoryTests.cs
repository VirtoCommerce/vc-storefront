using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using Moq;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.InventoryModuleApi.Models;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Stores;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Inventory
{
    public class InventoryTests
    {
        [Fact]
        // https://virtocommerce.atlassian.net/browse/VP-2011
        public async Task EvaluateProductInventories_NoDefaultFFCEntry_InventoryInfoFilled()
        {
            // Arrage
            var products = new[]
            {
                new Product() { Id = "Product1" }
            };
            var workContext = new WorkContext()
            {
                CurrentStore = new Store()
                {
                    DefaultFulfillmentCenterId = "FFC_Default",
                    AvailFulfillmentCenterIds = new[] { "FFC_Available_1", "FFC_Available_2" },
                },
            };
            var availableInventories = new List<InventoryInfo>()
            {
                new InventoryInfo() { ProductId = "Product1", FulfillmentCenterId = "FFC_Available_1", InStockQuantity = 3 },
                new InventoryInfo() { ProductId = "Product1", FulfillmentCenterId = "FFC_Available_2", InStockQuantity = 5 },
            };
            var inventoryService = CreateInventoryService(availableInventories);

            // Act
            await inventoryService.EvaluateProductInventoriesAsync(products, workContext);

            // Assert
            Assert.NotNull(products[0].Inventory);
            Assert.Equal("FFC_Available_2", products[0].Inventory.FulfillmentCenterId);
            Assert.Equal("Product1", products[0].Inventory.ProductId);
            Assert.Equal(5, products[0].Inventory.InStockQuantity);

            Assert.NotNull(products[0].InventoryAll);
            Assert.Equal(2, products[0].InventoryAll.Count);
            Assert.Equal("FFC_Available_1", products[0].InventoryAll[0].FulfillmentCenterId);
            Assert.Equal("Product1", products[0].InventoryAll[0].ProductId);
            Assert.Equal(3, products[0].InventoryAll[0].InStockQuantity);
            Assert.Equal("FFC_Available_2", products[0].InventoryAll[1].FulfillmentCenterId);
            Assert.Equal("Product1", products[0].InventoryAll[1].ProductId);
            Assert.Equal(5, products[0].InventoryAll[1].InStockQuantity);
        }

        private static InventoryService CreateInventoryService(IEnumerable<InventoryInfo> allInventoryInfos)
        {
            var inventoryModuleStub = new Mock<IInventoryModule>();

            inventoryModuleStub.Setup(x => x.GetProductsInventoriesByPlentyIdsWithHttpMessagesAsync(It.IsAny<IList<string>>(), null, It.IsAny<CancellationToken>()))
                .Returns<IList<string>, Dictionary<string, List<string>>, CancellationToken>((ids, customHeaders, cancellationToken) =>
                    Task.FromResult(
                        new HttpOperationResponse<IList<InventoryInfo>>()
                        {
                            Body = allInventoryInfos.Where(inventoryInfo => ids.Any(id => id == inventoryInfo.ProductId)).ToList(),
                        }));

            var storefrontMemoryCache = CreateStorefrontMemoryCache();

            var apiChangesWatcherStub = new Mock<IApiChangesWatcher>();
            var result = new InventoryService(inventoryModuleStub.Object, storefrontMemoryCache, apiChangesWatcherStub.Object);

            return result;
        }

        private static StorefrontMemoryCache CreateStorefrontMemoryCache()
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();

            var loggerFactoryStub = new Mock<ILoggerFactory>();
            var workContextAccessorStub = new Mock<IWorkContextAccessor>();
            var result = new StorefrontMemoryCache(memoryCache, Options.Create(new StorefrontOptions()), loggerFactoryStub.Object, workContextAccessorStub.Object);

            return result;
        }
    }
}
