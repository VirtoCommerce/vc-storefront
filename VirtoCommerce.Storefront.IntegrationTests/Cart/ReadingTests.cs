using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using Xunit;

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
                settings.Converters.Add(new MoneyJsonConverter());
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                return settings;
            };

            AssertionOptions.AssertEquivalencyUsing(options => options
                .Excluding(c => c.SelectedMemberPath == "Customer.Id")
                .Excluding(c => c.SelectedMemberPath == "Currency.CultureName")
                .Excluding(c => c.SelectedMemberPath == "Currency.NumberFormat")
                .Excluding(c => c.SelectedMemberPath == "Currency.EnglishName")
                .Excluding(c => c.SelectedMemberInfo.Name == "CustomerId")
                .Excluding(c => c.SelectedMemberInfo.Name == "ExtendedPriceTotal")
                .Excluding(c => c.SelectedMemberInfo.Name == "ExtendedPriceTotalWithTax")
                .ComparingByMembers<ShoppingCart>()
                .ComparingByMembers<User>()
                .ComparingByMembers<Currency>()
            );

        }

        [Fact]
        public async Task GetCart_IfCartIsNotExistForUser_ShouldReturnNewCart()
        {
            //arrange
            var client = _factory.CreateClient();

            //act
            var result = await client.GetAsync<ShoppingCart>(TestEnvironment.GetCartEndPoint);

            //assert
            result.Should().BeEquivalentTo(GetNewCart());
        }

        [Fact]
        public void GetCart_IfCartExistForUser_ShouldReturnExistedCart()
        {
        }

        [Fact]
        public void GetCartItemsCount_IfCartHasNoItems_ShouldReturnZero()
        {
        }

        [Fact]
        public void GetCartItemsCount_IfCartHasItems_ShouldReturnExactItemsCount()
        {
        }

        private ShoppingCart GetNewCart()
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
    }
}
