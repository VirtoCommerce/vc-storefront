using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.Model.Catalog;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Tests
{
    [Trait("Category", "CI")]
    [CollectionDefinition(nameof(ApiCatalogControllerTests), DisableParallelization = true)]
    public class ApiCatalogControllerTests : IClassFixture<StorefrontApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private bool _isDisposed;

        public ApiCatalogControllerTests(StorefrontApplicationFactory applicationFactory)
        {
            _client = applicationFactory.CreateClient();
        }

        [Fact]
        public async Task SearchProducts_GetProducts()
        {
            // Arrange
            var searchCriteria = new ProductSearchCriteria
            {
                Keyword = Infrastructure.Product.OctocopterSku,
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(searchCriteria),
                Encoding.UTF8,
                "application/json"
                );

            // Act
            var response = await _client
                .GotoMainPage()
                .PostAsync(TestEnvironment.CatalogSearchProductsEndpoint, content);

            var source = await response.Content.ReadAsStringAsync();

            var jobj = JObject.Parse(source).RemovePropertyInChildren(
                new[] { "$.products" },
                new[] { "properties", "variationProperties", "price", "prices" }
            );
            var products = jobj["products"]?.ToString() ?? string.Empty;

            var result = JsonConvert.DeserializeObject<List<Model.Catalog.Product>>(products);
            var product = result.FirstOrDefault();

            // Assert
            Assert.NotNull(product);
            Assert.NotEmpty(product.Id);
            Assert.NotEmpty(product.Name);
            Assert.NotEmpty(product.CatalogId);
            Assert.NotEmpty(product.CategoryId);
            Assert.Equal(Infrastructure.Product.OctocopterSku, product.Sku);
        }

        [Fact]
        public async Task GetProductsByIds_GetProducts()
        {
            // Arrange
            var productIds = new[]
            {
                Infrastructure.Product.Octocopter,
                Infrastructure.Product.Quadcopter,
            };

            // Act
            var response = await _client.GetAsync(TestEnvironment.CatalogGetProductsByIdsEndpoint(productIds));

            var source = await response.Content.ReadAsStringAsync();

            var result = JArray.Parse(source)
                .RemovePropertyInChildren(
                    new[] { "$" },
                    new[] { "properties", "variationProperties", "price", "prices", "descriptions" }
                )
                .ToString();
            var products = JsonConvert.DeserializeObject<List<Model.Catalog.Product>>(result);

            var quadrocopter = products.FirstOrDefault(x => x.Name.Contains("quadcopter", StringComparison.InvariantCultureIgnoreCase));
            var octocopter = products.FirstOrDefault(x => x.Name.Contains("octocopter", StringComparison.InvariantCultureIgnoreCase));

            // Assert
            Assert.NotNull(quadrocopter);
            Assert.NotNull(octocopter);
            Assert.Equal(Infrastructure.Product.Quadcopter, quadrocopter.Id);
            Assert.Equal(Infrastructure.Product.Octocopter, octocopter.Id);
            Assert.NotEmpty(quadrocopter.Name);
            Assert.NotEmpty(octocopter.Name);
            Assert.NotEmpty(quadrocopter.CatalogId);
            Assert.NotEmpty(octocopter.CatalogId);
            Assert.NotEmpty(quadrocopter.CategoryId);
            Assert.NotEmpty(octocopter.CategoryId);
        }

        [Fact]
        public async Task SearchCategories_GetCategories()
        {
            // Arrange
            var searchCriteria = new CategorySearchCriteria
            {
                Keyword = Infrastructure.Category.CopterCategoryCode,
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(searchCriteria),
                Encoding.UTF8,
                "application/json"
                );

            // Act
            var response = await _client
                .GotoMainPage()
                .PostAsync(TestEnvironment.CatalogSearchCategoriesEndpoint, content);

            var source = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(source).RemovePropertyInChildren(
                new[] { "$.categories" },
                new[] { "properties", "categories" }
                );
            var categories = jObject["categories"]?.ToString() ?? string.Empty;

            var result = JsonConvert.DeserializeObject<List<Model.Catalog.Category>>(categories);

            var category = result.FirstOrDefault();

            // Assert
            Assert.NotNull(category);
            Assert.NotEmpty(category.Id);
            Assert.NotEmpty(category.Name);
            Assert.Equal(Infrastructure.Category.CopterCategoryCode, category.Code);
            Assert.NotEmpty(category.ParentId);
        }

        [Fact]
        public async Task GetCategoriesByIds_GetCategories()
        {
            // Arrange
            var categoryIds = new[]
            {
                Infrastructure.Category.CopterCategoryId,
            };

            // Act
            var response = await _client.GetAsync(TestEnvironment.CatalogGetCategoriesByIdsEndpoint(categoryIds));

            var source = await response.Content.ReadAsStringAsync();

            var result = JArray.Parse(source).RemovePropertyInChildren(
                new[] { "$" },
                new[] { "properties", "categories" }
                )
                .ToString();
            var categories = JsonConvert.DeserializeObject<List<Model.Catalog.Category>>(result);
            var category = categories.FirstOrDefault();

            // Assert
            Assert.NotNull(category);
            Assert.Equal(Infrastructure.Category.CopterCategoryId, category.Id);
            Assert.NotEmpty(category.Code);
            Assert.NotEmpty(category.Name);
            Assert.NotEmpty(category.CatalogId);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _client?.Dispose();
            }

            _isDisposed = true;
        }

        ~ApiCatalogControllerTests()
        {
            Dispose(true);
        }
    }
}
