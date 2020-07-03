using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi.Models;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Subscriptions;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Cart
{
    [Trait("Category", "CI")]
    [CollectionDefinition("ApiCartControllerTests", DisableParallelization = true)]
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

            if (disposing)
            {
                _client?.Dispose();
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
            var result = await _client.GetCart();

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
            var result = await _client.GetCart();

            //assert
            GetCartComparationResult(
                result,
                "GetFilledCartWithItem",
                new[] { "$", "$.items[*]", "$.recentlyAddedItem", "$.items[*].product", "$.recentlyAddedItem.product" },
                new[] { "id", "primaryImage", "images", "imageUrl" })
                .Should()
                .BeNull();

            _client
                .ClearCart()
                .Logout();
        }

        // TODO: test should be improved, because of blinking (looks like comparation result waiting result with same order in items)
        [Fact]
        public async Task GetCart_IfAnonimousHaveItemsInCartAndLoggedIn_ShouldReturnMergedCart()
        {
            //arrange
            _client.Login("admin", "store");

            await RemoveAllCoupons();

            _client
                .ClearCart()
                .Logout()
                .GotoMainPage()
                .ClearCart()
                .InsertCartItem(new AddCartItem { Id = Product.Quadcopter, Quantity = 1 })
                .Login("admin", "store")
                .InsertCartItem(new AddCartItem { Id = Product.Octocopter, Quantity = 1 });

            //act
            var result = await _client.GetCart();

            //assert
            GetCartComparationResult(
                    result,
                    "GetMergedAnonymousCartWithAdmin",
                    new[] { "$", "$.items[*]", "$.recentlyAddedItem", "$.items[*].product", "$.recentlyAddedItem.product" },
                    new[] { "id", "primaryImage", "images", "imageUrl" })
                .Should()
                .BeNull();

            _client
                .ClearCart()
                .Logout()
                .GotoMainPage()
                .ClearCart();
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
            var quantity = new Fixture().Create<int>();

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
                .Login("admin", "store");

            await RemoveAllCoupons();

            _client
                .ClearCart();

            //act
            var result = await _client.GetCart();

            //assert
            GetCartComparationResult(result, "GetEmptyCartForAdmin", new[] { "$" }, new[] { "id" })
                .Should()
                .BeNull();

            _client.Logout();
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
                Quantity = new Fixture().Create<int>()
            };
            var content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                "application/json");

            //act
            var response = await _client.PostAsync<ShoppingCartItems>(TestEnvironment.CartItemsEndpoint, content);

            //assert
            response.Should().BeEquivalentTo(new ShoppingCartItems { ItemsCount = item.Quantity });
            _client.ClearCart().Logout();
        }

        [Fact]
        public async Task AddItemToCart_IfItemIsNotAvailable_ShouldNotUpdateCartItems()
        {
            //arrange
            _client
                .GotoMainPage()
                .ClearCart();

            var randomizer = new Fixture();

            var newItem = new AddCartItem
            {
                Id = randomizer.Create<string>(),
                Quantity = randomizer.Create<int>()
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(newItem),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync<ShoppingCartItems>(TestEnvironment.CartItemsEndpoint, content);

            //act
            var result = _client.InsertCartItem(newItem);

            //assert
            response.Should().BeEquivalentTo(new ShoppingCartItems { ItemsCount = 0 });
            _client.ClearCart();
        }

        [Fact]
        public async Task DeleteItemFromCart_ShouldReturnCartItemsCount()
        {
            //arrange
            _client
                .GotoMainPage()
                .ClearCart()
                .InsertCartItem(new AddCartItem() { Id = Product.Quadcopter, Quantity = 1 })
                .InsertCartItem(new AddCartItem() { Id = Product.Octocopter, Quantity = 1 });

            var cart = await _client.GetCart();
            var lineItemIds = GetLineItemIds(cart);

            //act
            _client.DeleteCartItem(lineItemIds.FirstOrDefault());

            //assert
            var updatedCart = await _client.GetCart();

            lineItemIds.Count().Should().Be(2);
            GetLineItemIds(updatedCart).Count().Should().Be(1);

            _client.ClearCart();
        }

        [Fact]
        public async Task ChangeCartItemPrice_IfPriceIsMoreThanCurrent_ShouldUpdatePriceForItem()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart()
                .InsertCartItem(new AddCartItem { ProductId = Product.Quadcopter, Quantity = 1 });

            var cart = await _client.GetCart();
            var itemPriceDetail = GetCurrentPriceAmountOfLineItem(Product.Quadcopter, cart);
            var newPrice = itemPriceDetail.listPrice + new Fixture().Create<decimal>();

            //act
            _client.ChangeCartItemPrice(new ChangeCartItemPrice
            {
                LineItemId = itemPriceDetail.lineItemId,
                NewPrice = newPrice
            });

            //assert
            var updatedCart = await _client.GetCart();
            var updatedPriceDetail = GetCurrentPriceAmountOfLineItem(Product.Quadcopter, updatedCart);

            newPrice.Should().BeGreaterThan(itemPriceDetail.listPrice);
            updatedPriceDetail.listPrice.Should().Be(newPrice);
            updatedPriceDetail.salePrice.Should().Be(newPrice);

            _client.ClearCart().Logout();
        }

        //TODO: looks like fluent validation rule for changing item price to less value doesn't work properly
        [Fact]
        public async Task ChangeCartItemPrice_IfPriceIsLessThanCurrent_ShouldNotUpdatePriceForItem()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart()
                .InsertCartItem(new AddCartItem { ProductId = Product.Quadcopter, Quantity = 1 });

            var cart = await _client.GetCart();
            var itemPriceDetail = GetCurrentPriceAmountOfLineItem(Product.Quadcopter, cart);
            var newPrice = itemPriceDetail.listPrice - new Fixture().Create<decimal>();

            //act
            _client.ChangeCartItemPrice(new ChangeCartItemPrice
            {
                LineItemId = itemPriceDetail.lineItemId,
                NewPrice = newPrice
            });

            //assert
            var updatedCart = await _client.GetCart();
            var updatedPriceDetail = GetCurrentPriceAmountOfLineItem(Product.Quadcopter, updatedCart);

            newPrice.Should().BeLessThan(itemPriceDetail.listPrice);
            updatedPriceDetail.listPrice.Should().Be(itemPriceDetail.listPrice);
            updatedPriceDetail.salePrice.Should().Be(itemPriceDetail.salePrice);

            _client.ClearCart().Logout();
        }

        [Fact]
        public async Task GetCartShipmentAvailShippingMethods_ShouldReturnShipmentMethods()
        {
            //arrange
            _client
                .Login("admin", "store");
            var shipmentId = new Fixture().Create<string>(); //TODO: looks like it's unused in Storefront

            //act
            var result = await _client.GetStringAsync(TestEnvironment.ShippingMethodsEndpoint(shipmentId));

            //assert
            GetCartComparationResult(result, "GetShipmentMethodsForAdmin")
                .Should()
                .BeNull();

            _client.Logout();
        }

        [Fact]
        public async Task GetCartAvailPaymentMethods_ShouldReturnPaymentMethods()
        {
            //arrange
            _client
                .Login("admin", "store");

            //act
            var result = await _client.GetStringAsync(TestEnvironment.PaymentMethodsEndpoint);

            //assert
            GetCartComparationResult(result, "GetPaymentMethodsForAdmin")
                .Should()
                .BeNull();

            _client.Logout();
        }

        [Fact]
        public async Task AddCartCoupon_ShouldAddCouponToCart()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            await RemoveAllCoupons();

            var couponCode = new Fixture().Create<string>();

            //act
            _client.AddCoupon(couponCode);

            //assert
            var cart = await _client.GetCart();

            var couponCodes = GetCouponCodes(cart);

            couponCodes.Should().BeEquivalentTo(new[] { couponCode }.ToList());

            _client.ClearCart().Logout();
        }

        [Fact]
        public async Task RemoveCartCoupon_ShouldRemoveCouponFromCart()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            await RemoveAllCoupons();

            var couponCode = new Fixture().Create<string>();
            _client.AddCoupon(couponCode);

            //act
            _client.RemoveCoupon(couponCode);

            //assert
            var cart = await _client.GetCart();

            GetCouponCodes(cart).Should().BeNullOrEmpty();

            _client.Logout();
        }

        [Fact]
        public async Task AddOrUpdateCartPaymentPlan_ShouldUpdatePaymentPlan()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            var paymentPlan = new Fixture().Create<PaymentPlan>();

            //act
            _client.AddOrUpdateCartPaymentPlan(paymentPlan);

            //assert
            var cart = await _client.GetCart();

            var paymentPlanJson = GetPaymentPlan(cart);
            ComparationResult(paymentPlanJson, JsonConvert.SerializeObject(paymentPlan), new[] { "$" }, new[] { "id" });

            _client
                .DeleteCartPaymentPlan()
                .ClearCart()
                .Logout();
        }

        [Fact]
        public async Task DeleteCartPaymentPlan_ShouldRemovePaymentPlan()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            var paymentPlan = new Fixture().Create<PaymentPlan>();
            _client.AddOrUpdateCartPaymentPlan(paymentPlan);

            //act
            _client.DeleteCartPaymentPlan();

            //assert
            var cart = await _client.GetCart();

            GetPaymentPlan(cart).Should().BeNull();

            _client
                .ClearCart()
                .Logout();
        }

        [Fact]
        public async Task AddOrUpdateCartShipment_IfShipmentIsRegistered_ShouldAddCartShipment()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            //act
            var response = await _client.AddOrUpdateCartShipment(TestRequestEnvironment.ShipmentIsRegistered);
            var content = await response.Content.ReadAsStringAsync();

            //assert
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get failed: {response.StatusCode} {content}");
            }

            response.IsSuccessStatusCode.Should().BeTrue();

            _client
                .ClearCart()
                .Logout();
        }

        [Fact]
        public async Task AddOrUpdateCartShipment_IfShipmentIsNotRegistered_ShouldReturnBadRequest()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            //act
            var response = await _client.AddOrUpdateCartShipment(TestRequestEnvironment.ShipmentIsNotRegistered);

            //assert
            response.IsSuccessStatusCode.Should().BeFalse();

            _client
                .ClearCart()
                .Logout();
        }

        [Fact]
        public async Task AddOrUpdateCartPayment_IfPaymentIsRegistered_ShouldAddCartPayment()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            //act
            var response = await _client.AddOrUpdateCartPayment(TestRequestEnvironment.PaymentIsRegistered);

            //assert
            response.IsSuccessStatusCode.Should().BeTrue();

            _client
                .ClearCart()
                .Logout();
        }

        [Fact]
        public async Task AddOrUpdateCartPayment_IfPaymentIsNotRegistered_ShouldReturnBadRequest()
        {
            //arrange
            _client
                .Login("admin", "store")
                .ClearCart();

            //act
            var response = await _client.AddOrUpdateCartPayment(TestRequestEnvironment.PaymentIsNotRegistered);

            //assert
            response.IsSuccessStatusCode.Should().BeFalse();

            _client
                .ClearCart()
                .Logout();
        }


        private T GetRequestDataFromFile<T>(string fileNameWithExpectation) where T : class
        {
            var response = File.ReadAllText($"Requests\\{fileNameWithExpectation}.json");

            if (string.IsNullOrEmpty(response))
            {
                throw new Exception($"Get request data from file {fileNameWithExpectation} failed");
            }

            var jobject = JObject.Parse(response);

            return jobject.ToObject<T>();
        }

        private string GetRequestDataFromFile(string fileNameWithExpectation)
        {
            var response = File.ReadAllText($"Requests\\{fileNameWithExpectation}.json");

            if (string.IsNullOrEmpty(response))
            {
                throw new Exception($"Get request data from file {fileNameWithExpectation} failed");
            }

            return response;
        }

        private string GetCartComparationResult(string actualJson, string fileNameWithExpectation, IEnumerable<string> pathsForExclusion = null, IEnumerable<string> excludedProperties = null)
        {
            var expectedResponse = File.ReadAllText($"Responses\\{fileNameWithExpectation}.json");
            return ComparationResult(actualJson, expectedResponse, pathsForExclusion, excludedProperties);
        }

        private string ComparationResult(string actualJson, string expectedJson, IEnumerable<string> pathsForExclusion = null, IEnumerable<string> excludedProperties = null)
        {
            var actualResult = JToken.Parse(actualJson).RemovePropertyInChildren(pathsForExclusion, excludedProperties).ToString();
            var expectedResult = JToken.Parse(expectedJson).RemovePropertyInChildren(pathsForExclusion, excludedProperties).ToString();
            return new JsonDiffPatch().Diff(actualResult, expectedResult);
        }

        private (string lineItemId, decimal listPrice, decimal salePrice) GetCurrentPriceAmountOfLineItem(string productId, string serializedCart)
        {
            var cartObject = JObject.Parse(serializedCart);

            var token = cartObject["items"].Children().FirstOrDefault(c => c["productId"].Value<string>() == productId);
            if (token == null)
            {
                throw new Exception($"Item with product id {productId} not found");
            }

            return (
                lineItemId: token["id"].Value<string>(),
                listPrice: token["listPrice"]["amount"].Value<decimal>(),
                salePrice: token["salePrice"]["amount"].Value<decimal>());
        }

        private IList<string> GetCouponCodes(string actualJson)
        {
            var cartObject = JObject.Parse(actualJson);
            return cartObject["coupons"].Children()["code"].Values<string>().ToList();
        }

        private string GetPaymentPlan(string actualJson)
        {
            var cartObject = JObject.Parse(actualJson);
            return cartObject["paymentPlan"]?.ToString();
        }

        private ShipmentMethod GetShippingMethod(string actualJson)
        {
            var cartObject = JArray.Parse(actualJson);
            return cartObject.FirstOrDefault().ToObject<ShipmentMethod>();
        }

        private string[] GetLineItemIds(string actualJson)
        {
            var cartObject = JObject.Parse(actualJson);
            return cartObject["items"]?.Children()["id"].Select(x => x.ToString()).ToArray();
        }

        private async Task RemoveAllCoupons()
        {
            var cart = await _client.GetCart();

            var couponCodes = GetCouponCodes(cart);

            foreach (var couponCode in couponCodes)
            {
                _client.RemoveCoupon(couponCode);
            }
        }
    }
}
