using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.IntegrationTests.Models;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using Xunit;
using MoneyJsonConverter = VirtoCommerce.Storefront.IntegrationTests.Infrastructure.MoneyJsonConverter;

namespace VirtoCommerce.Storefront.IntegrationTests.Cart
{
    [Trait("Category", "CI")]
    public class ReadingTests : IClassFixture<StorefrontApplicationFactory>
    {
        private readonly StorefrontApplicationFactory _factory;

        public ReadingTests(StorefrontApplicationFactory factory)
        {
            _factory = factory;

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                //settings.Converters.Add(new UserBackwardCompatibilityJsonConverter(settings));
                settings.Converters.Add(new MoneyJsonConverter());
                //settings.Converters.Add(new CatalogPropertyListAsArrayJsonConverter());
                settings.Converters.Add(new MutablePagedListJsonConverter<CatalogProperty>());
                settings.Converters.Add(new MutablePagedListJsonConverter<DynamicProperty>());
                settings.Converters.Add(new TierPriceJsonConverter());
                settings.Converters.Add(new CurrencyJsonConverter());

                return settings;
            };

            AssertionOptions.AssertEquivalencyUsing(options => options
                .Excluding(c => c.SelectedMemberPath == "Customer.Id")
                .Excluding(c => c.SelectedMemberPath == "Currency.CultureName")
                .Excluding(c => c.SelectedMemberPath == "Currency.NumberFormat")
                .Excluding(c => c.SelectedMemberPath == "Currency.EnglishName")
                .Excluding(c => c.SelectedMemberInfo.MemberType == typeof(TierPrice))
                //.Excluding(c => Regex.IsMatch(c.SelectedMemberPath, "Items\\[.+\\].Product"))
                .Excluding(c => c.SelectedMemberInfo.Name == "CustomerId")
                .Excluding(c => c.SelectedMemberInfo.Name == "ExtendedPriceTotal")
                .Excluding(c => c.SelectedMemberInfo.Name == "ExtendedPriceTotalWithTax")
                .ComparingByMembers<ShoppingCart>()
                .ComparingByMembers<User>()
                .ComparingByMembers<Currency>()
                .IgnoringCyclicReferences()
            );
        }

        [Fact]
        public async Task GetCart_IfCartIsNotExistForUser_ShouldReturnNewCart()
        {
            //arrange
            var client = _factory.CreateClient();

            //act
            var result = await client.GetAsync<ShoppingCart>(TestEnvironment.CartEndpoint);

            //assert
            result.Should().BeEquivalentTo(GetEmptyCartForAnonimousUser());
        }

        [Fact]
        public async Task GetCart_IfCartExistForUser_ShouldReturnExistedCart()
        {

            using (var client = _factory.CreateClient())
            {
                //arrange
                var cookie = await Login(client);
                await ClearCart(client, cookie);
                var item = new AddCartItem { Id = "9cbd8f316e254a679ba34a900fccb076", Quantity = 1 };

                await FillCartWithItem(client, cookie, item);

                //act
                var result = await GetCart(client);

                //assert
                result.Should().BeEquivalentTo(JToken.Parse(ApiResponse.FilledCartWithItem));
                //result.Should().BeEquivalentTo(GetFilledCartForAdmin());
                await Logout(client);
            }
        }

        [Fact]
        public void GetCartItemsCount_IfCartHasNoItems_ShouldReturnZero()
        {
        }

        [Fact]
        public void GetCartItemsCount_IfCartHasItems_ShouldReturnExactItemsCount()
        {
        }

        private async Task<AntiforgeryCookie> Login(HttpClient client)
        {
            var loginPage = await client.GetAsync("account/login");

            var antiforgery = new AntiforgeryCookie(loginPage.Headers);
            var content = new MultipartFormDataContent
            {
                { new StringContent("admin"), "customer[user_name]" },
                { new StringContent("store"), "customer[password]" },
                { new StringContent(antiforgery.Value), "__RequestVerificationToken" }
            };

            var loginResponse = await client.PostAsync("account/login", content);
            if (!loginResponse.IsSuccessStatusCode && loginResponse.StatusCode != HttpStatusCode.Redirect)
            {
                throw new Exception($"Login failed: {loginResponse.StatusCode}");
            }
            var cookie = new AntiforgeryCookie(loginResponse.Headers);
            client.DefaultRequestHeaders.Add(cookie.Key, new[] { cookie.Value });

            return cookie;
        }

        private async Task FillCartWithItem(HttpClient client, AntiforgeryCookie cookie,
            AddCartItem item)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(TestEnvironment.CartItemsEndpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Fill cart failed: {response.StatusCode}");
            }
        }

        private async Task<JToken> GetCart(HttpClient client)
        {
            //var cartItemsCountResponse = await client.GetAsync(TestEnvironment.CartItemsCountEndpoint);
            //if (!cartItemsCountResponse.IsSuccessStatusCode)
            //{
            //    throw new Exception($"Get cart items count failed: {cartItemsCountResponse.StatusCode}");
            //}

            var response = await client.GetAsync(TestEnvironment.CartEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get cart failed: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return JToken.Parse(result);
            //return JsonConvert.DeserializeObject<ShoppingCart>(await response.Content.ReadAsStringAsync());
        }

        private async Task ClearCart(HttpClient client, AntiforgeryCookie cookie)
        {
            var response = await client.PostAsync(TestEnvironment.CartClearEndpoint, new StringContent(""));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Clear cart failed: {response.StatusCode}");
            }
        }

        private async Task Logout(HttpClient client)
        {
            var response = await client.GetAsync(TestEnvironment.LogoutEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Logout failed: {response.StatusCode}");
            }
        }

        private ShoppingCart GetEmptyCartForAnonimousUser()
        {
            var newCustomer = new User
            {
                UserName = "Anonymous"
            };
            var cart = new ShoppingCart(
                new Currency(
                    new Language(TestEnvironment.DefaultCultureName),
                    TestEnvironment.DefaultCurrencyCode,
                    TestEnvironment.DefaultCurrencyName,
                    TestEnvironment.DefaultCurrencySymbol,
                    TestEnvironment.DefaultExchangeRate),
                new Language(TestEnvironment.DefaultCultureName))
            {
                Name = "default",
                StoreId = "Electronics",
                IsAnonymous = true,
                CustomerName = "Anonymous",
                Customer = newCustomer,
            };

            return cart;
        }

        private ShoppingCart GetFilledCartForAdmin()
        {
            var newCustomer = new User
            {
                UserName = "admin",
                Email = "admin@vc-demostore.com",
                IsRegisteredUser = true,
                IsAdministrator = true,
                UserType = "Administrator",
                Permissions = new string[0],
                IsFirstTimeBuyer = true,
            };
            var language = new Language("en-US");
            var currency = new Currency(language, "USD");
            var price = new Money(995.99m, currency);
            var cart = new ShoppingCart(
                new Currency(
                    new Language(TestEnvironment.DefaultCultureName),
                    TestEnvironment.DefaultCurrencyCode,
                    TestEnvironment.DefaultCurrencyName,
                    TestEnvironment.DefaultCurrencySymbol,
                    TestEnvironment.DefaultExchangeRate),
                new Language(TestEnvironment.DefaultCultureName))
            {
                Name = "default",
                StoreId = "Electronics",
                IsAnonymous = false,
                CustomerName = "admin",
                Customer = newCustomer,
                Items = new List<LineItem>()
                {
                    new LineItem(currency, language)
                    {
                        CatalogId = "4974648a41df4e6ea67ef2ad76d7bbd4",
                        CategoryId = "e51b5f9eea094a44939c11d4d4fa3bb1",
                        CreatedDate = DateTime.MinValue,
                        ExtendedPrice = price,
                        ExtendedPriceWithTax = price,
                        Id = "8446b766-2b16-4706-999b-09644ccc9b0c",
                        ImageUrl = @"//localhost:10645/assets/catalog/3DRSOLO/1428965138000_1133723.jpg",
                        InStockQuantity = 7,

                    }
                },
                ObjectType = typeof(ShoppingCart).Name,
                Id = "494300ab-39ed-4eab-8ebc-d9ff5a79dbba"
            };

            return cart;
        }
    }
}
