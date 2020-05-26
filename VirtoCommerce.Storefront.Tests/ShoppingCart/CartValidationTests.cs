using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using FluentValidation;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Inventory
{
    public class CartValidationTests
    {
        [Fact]       
        public async Task CartValidation()
        {
            //TODO: Add unit tests

            //Arrange
            var cartService = new Moq.Mock<ICartService>();
            cartService.Setup(x => x.GetAvailableShippingMethodsAsync(It.IsAny<ShoppingCart>())).Returns(Task.FromResult(new []
            {
                 new ShippingMethod
                 {
                     ShipmentMethodCode = "USPS"
                 }
            } as IEnumerable<ShippingMethod>));
            //Act
            var validator = new CartValidator(cartService.Object);

            var result = await validator.ValidateAsync(GetCart(), ruleSet: "unstrict" );
            //Assertion

        }


        private ShoppingCart GetCart()
        {
            var usd = new Currency(Language.InvariantLanguage, "USD");
            return new ShoppingCart(usd, Language.InvariantLanguage)
            {
                Items = new[]
                {
                    new LineItem(usd, Language.InvariantLanguage)
                    {
                        Product = new Product
                        {
                            IsActive = false
                        }
                    }
                },
                Shipments = new []
                {
                    new Shipment
                    {
                        ShipmentMethodCode = "FedEx"
                    }
                }
            };
        }
  
    }
}
