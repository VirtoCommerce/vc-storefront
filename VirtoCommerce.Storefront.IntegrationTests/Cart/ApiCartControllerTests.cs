using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.IntegrationTests.Models;
using VirtoCommerce.Storefront.Model.Cart;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Cart
{
    [Trait("Category", "CI")]
    public class ApiCartControllerTests : IClassFixture<StorefrontApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private bool _isDisposed;

        public ApiCartControllerTests(StorefrontApplicationFactory factory)
        {
            _client = factory.CreateClient();
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

            if (disposing && _client != null)
            {
                if (_client.DefaultRequestHeaders.TryGetValues(AntiforgeryCookie.Name, out var tokens))
                {
                    _client.ClearCart();
                }

                _client.Dispose();
            }

            _isDisposed = true;
        }

        ~ApiCartControllerTests()
        {
            Dispose(false);
        }

        [Fact]
        public async Task GetCart_IfCartIsNotExistForUser_ShouldReturnNewCart()
        {
            //act
            var result = await GetCart(_client);

            //assert
            GetCartComparationResult(
                result,
                "GetEmptyCartForAnonymous",
                new[] { "$.customer", "$" },
                new[] { "id", "customerId" })
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task GetCart_IfCartExistForUser_ShouldReturnExistedCart()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart()
                .InsertCartItem(new AddCartItem { Id = Product.Quadcopter, Quantity = 1 });

            //act
            var result = await GetCart(_client);

            //assert
            GetCartComparationResult(
                result,
                "GetFilledCartWithItem",
                new[] { "$.items[*]", "$.recentlyAddedItem" },
                new[] { "id" })
                .Should()
                .BeNull();

            _client.ClearCart().Logout();
        }

        [Fact]
        public async Task GetCart_IfAnonimousHaveItemsInCartAndLoggedIn_ShouldReturnMergedCart()
        {
            //arrange
            _client
                .GotoMainPage()
                .ClearCart()
                .InsertCartItem(new AddCartItem { Id = Product.Quadcopter, Quantity = 1 })
                .Login("admin", "store")
                .InsertCartItem(new AddCartItem { Id = Product.Octocopter, Quantity = 1 });

            //act
            var result = await GetCart(_client);

            //assert
            GetCartComparationResult(
                    result,
                    "GetMergedAnonymousCartWithAdmin",
                    new[] { "$.items[*]", "$.recentlyAddedItem" },
                    new[] { "id" })
                .Should()
                .BeNull();

            _client.ClearCart().Logout();
        }

        [Fact]
        public async Task GetCartItemsCount_IfCartHasNoItems_ShouldReturnZero()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            //act
            var result = await _client.GetAsync<int>(TestEnvironment.CartItemsCountEndpoint);

            //assert
            result.Should().Be(0);
            _client.Logout();
        }

        [Fact]
        public async Task GetCartItemsCount_IfCartHasItems_ShouldReturnExactItemsCount()
        {
            //arrange
            var quantity = (new Fixture()).Create<int>();

            _client
                .Login("admin", "store")
                .ClearCart()
                .InsertCartItem(new AddCartItem { Id = Product.Quadcopter, Quantity = quantity });

            //act
            var result = await _client.GetAsync<int>(TestEnvironment.CartItemsCountEndpoint);

            //assert
            result.Should().Be(quantity);
            _client.ClearCart().Logout();
        }

        [Fact]
        public async Task ClearCart_IfCartExists_ShouldReturnEmptyCart()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            //act
            var result = await GetCart(_client);

            //assert
            GetCartComparationResult(result, "GetEmptyCartForAdmin")
                .Should()
                .BeNull();
        }

        [Fact]
        public async Task AddItemToCart_IfItemIsAvailable_ShouldReturnCartItemsCount()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            var item = new AddCartItem
            {
                Id = Product.Quadcopter,
                Quantity = (new Fixture()).Create<int>()
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                "application/json");

            //act
            var response = await _client.PostAsync<ShoppingCartItems>(TestEnvironment.CartItemsEndpoint, content);

            //assert
            response.Should().BeEquivalentTo(new ShoppingCartItems { ItemsCount = item.Quantity });
            _client.ClearCart();
        }

        [Fact]
        public async Task AddItemToCart_IfItemIsNotAvailable_ShouldNotUpdateCartItems()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ChangeCartItemPrice_IfPriceIsMoreThanCurrent_ShouldUpdatePriceForItem()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ChangeCartItemPrice_IfPriceIsLessThanCurrent_ShouldNotUpdatePriceForItem()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddCartCoupon_ShouldAddCouponToCart()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task RemoveCartCoupon_ShouldRemoveCouponFromCart()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddOrUpdateCartPaymentPlan_ShouldUpdatePaymentPlan()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task DeleteCartPaymentPlan_ShouldRemovePaymentPlan()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddOrUpdateCartShipment_IfShipmentIsRegistered_ShouldAddCartShipment()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddOrUpdateCartShipment_IfShipmentIsNotRegistered_ShouldReturnBadRequest()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task AddOrUpdateCartPayment_IfShipmentIsRegistered_ShouldAddCartPayment()
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetCart(HttpClient client)
        {
            var response = await client.GetAsync(TestEnvironment.CartEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get cart failed: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        private string GetCartComparationResult(string actualJson, string fileNameWithExpectation, IEnumerable<string> pathsForExclusion = null, IEnumerable<string> excludedProperties = null)
        {
            var expectedResponse = File.ReadAllText($"Responses\\{fileNameWithExpectation}.json");
            var actualResult = JToken.Parse(actualJson).RemovePropertyInChildren(pathsForExclusion, excludedProperties).ToString();
            var expectedResult = JToken.Parse(expectedResponse).RemovePropertyInChildren(pathsForExclusion, excludedProperties).ToString();
            return new JsonDiffPatch().Diff(actualResult, expectedResult);
        }
    }
}
