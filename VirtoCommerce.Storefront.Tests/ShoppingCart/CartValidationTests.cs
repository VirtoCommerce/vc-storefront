using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Catalog;
using inventory = VirtoCommerce.Storefront.Model.Inventory;
using VirtoCommerce.Storefront.Model.Common;
using FluentValidation;
using Xunit;
using Bogus;
using System;
using System.Linq;
using FluentValidation.Validators;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;

namespace VirtoCommerce.Storefront.Tests.ShopingCart
{
    public class CartValidationTests
    {
        const string CART_NAME = "default";
        const string CURRENCY_CODE = "USD";
        const int InStockQuantity = 100;

        static readonly string[] ShipmentMehodCodes = new[] { "FedEx", "DHL", "EMS" };

        static readonly Currency Usd = new Currency(Language.InvariantLanguage, CURRENCY_CODE);

        static readonly IEnumerable<ShippingMethod> ShippingMethods = GetShippingMethods();

        static readonly Randomizer Rand = new Randomizer();
        static readonly Faker Faker = new Faker();

        [Fact]       
        public async Task ValidateCart_RuleSetDefault_Valid()
        {
            //Arrange
            var cartService = new Moq.Mock<ICartService>();

            //Act
            var validator = new CartValidator(cartService.Object);
            var cart = GetValidCart();
            var result = await validator.ValidateAsync(cart, ruleSet: "default" );

            //Assertion
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            //Assert.Empty(cart.ValidationErrors);


        }

        [Fact]
        public async Task ValidateCart_RuleSetDefault_Invalid()
        {

            //Arrange
            var cartService = new Moq.Mock<ICartService>();
            
            //Act
            var validator = new CartValidator(cartService.Object);
            var cart = GetInvalidCart();
            var result = await validator.ValidateAsync(cart, ruleSet: "default");

            //Assertion
            Assert.False(result.IsValid);
            //Assert.NotEmpty(cart.ValidationErrors);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(4, result.Errors.Count);
            
            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(cart.Name), x.PropertyName); Assert.Equal(nameof(NotNullValidator), x.ErrorCode); }
                                           , x => { Assert.Equal(nameof(cart.Name), x.PropertyName); Assert.Equal(nameof(NotEmptyValidator), x.ErrorCode); }
                                           , x => { Assert.Equal(nameof(cart.CustomerId), x.PropertyName); Assert.Equal(nameof(NotNullValidator), x.ErrorCode); }
                                           , x => { Assert.Equal(nameof(cart.CustomerId), x.PropertyName); Assert.Equal(nameof(NotEmptyValidator), x.ErrorCode); }
                              );
        }
        
        [Fact]
        public async Task ValidateShipment_RuleSetStrict_Valid()
        {
            //Arrange
            var cartService = new Moq.Mock<ICartService>();
            cartService.Setup(x => x.GetAvailableShippingMethodsAsync(It.IsAny<ShoppingCart>())).Returns(Task.FromResult(ShippingMethods));
            var cart = GetValidCart();

            //Act
            var validator = new CartShipmentValidator(cart, cartService.Object);
            var shipmentForValidation = cart.Shipments[0];
            var result = await validator.ValidateAsync(shipmentForValidation, ruleSet: "strict");

            //Assertion
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Empty(shipmentForValidation.ValidationErrors);
        }

        [Fact]
        public async Task ValidateShipment_RuleSetStrict_UnavailableMethodError()
        {
            //Arrange
            var cartService = new Moq.Mock<ICartService>();
            cartService.Setup(x => x.GetAvailableShippingMethodsAsync(It.IsAny<ShoppingCart>())).Returns(Task.FromResult(ShippingMethods));
            var cart = GetValidCart();

            var testShipments = new Faker<Shipment>()
               .CustomInstantiator(f => new Shipment(Usd))
               .RuleFor(s => s.ShipmentMethodCode, f => f.Random.Guid().ToString());

            var unavailableShipment = testShipments.Generate();
            cart.Shipments.Add(unavailableShipment);

            //Act
            var validator = new CartShipmentValidator(cart, cartService.Object);
            var result = await validator.ValidateAsync(unavailableShipment, ruleSet: "strict");

            //Assertion
            Assert.False(result.IsValid);            
            Assert.Single(result.Errors);

            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(unavailableShipment.ShipmentMethodCode), x.PropertyName);
            });

            Assert.Single(unavailableShipment.ValidationErrors);

            Assert.Collection(unavailableShipment.ValidationErrors, x =>
            {
                Assert.Equal(nameof(UnavailableError), x.ErrorCode);
            });
        }

        [Fact]
        public async Task ValidateShipment_RuleSetStrict_PriceError()
        {
            //Arrange
            var cartService = new Moq.Mock<ICartService>();
            cartService.Setup(x => x.GetAvailableShippingMethodsAsync(It.IsAny<ShoppingCart>())).Returns(Task.FromResult(ShippingMethods));
            var cart = GetValidCart();
            var shipment = Faker.PickRandom( cart.Shipments );
            shipment.Price = new Money(shipment.Price.Amount+1m, Usd);
            cart.Shipments.Add(shipment);

            //Act
            var validator = new CartShipmentValidator(cart, cartService.Object);
            var result = await validator.ValidateAsync(shipment, ruleSet: "strict");

            //Assertion
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);

            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(shipment.Price), x.PropertyName);
            });

            Assert.Single(shipment.ValidationErrors);

            Assert.Collection(shipment.ValidationErrors, x =>
            {
                Assert.Equal(nameof(PriceError), x.ErrorCode);
            });
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetDefault_Valid()
        {
            //Arrange            
            var cart = GetValidCart();

            var item = Faker.PickRandom( cart.Items );
            
            var newItemPrice = new ChangeCartItemPrice
            {
                LineItemId = item.Id,
                NewPrice = Rand.Decimal(1, 100)
            };

            //Act
            var validator = new ChangeCartItemPriceValidator(cart);

            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "default");
            //Assertion
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);            
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetDefault_Invalid()
        {
            //Arrange            
            var cart = GetValidCart();            

            var newItemPrice = new ChangeCartItemPrice
            {
                LineItemId = null,
                NewPrice = 0
            };

            //Act
            var validator = new ChangeCartItemPriceValidator(cart);

            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "default");
            //Assertion
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Equal(3, result.Errors.Count);

            Assert.Collection(result.Errors, x => { Assert.Equal(nameof(newItemPrice.NewPrice), x.PropertyName); Assert.Equal(nameof(GreaterThanValidator), x.ErrorCode); }                                          
                                          , x => { Assert.Equal(nameof(newItemPrice.LineItemId), x.PropertyName); Assert.Equal(nameof(NotNullValidator), x.ErrorCode); }
                                          , x => { Assert.Equal(nameof(newItemPrice.LineItemId), x.PropertyName); Assert.Equal(nameof(NotEmptyValidator), x.ErrorCode); }
                             );
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetStrict_Valid()
        {
            //Arrange            
            var cart = GetValidCart();
            var item = Faker.PickRandom(cart.Items);

            var newItemPrice = new ChangeCartItemPrice
            {
                LineItemId = item.Id,
                NewPrice = item.ListPrice.Amount + Rand.Decimal(1, 100)
            };

            //Act
            var validator = new ChangeCartItemPriceValidator(cart);

            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "strict");
            //Assertion
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateChangePriceItem_RuleSetStrict_Invalid()
        {
            //Arrange            
            var cart = GetValidCart();
            var item = Faker.PickRandom(cart.Items);

            var newItemPrice = new ChangeCartItemPrice
            {
                LineItemId = item.Id,
                NewPrice = item.ListPrice.Amount - Rand.Decimal(0, item.ListPrice.Amount)
            };

            //Act
            var validator = new ChangeCartItemPriceValidator(cart);

            var result = await validator.ValidateAsync(newItemPrice, ruleSet: "strict");
            //Assertion
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Collection(result.Errors, x =>
            {
                Assert.Equal(nameof(item.SalePrice), x.PropertyName);
            });
        }

        private ShoppingCart GetValidCart()
        {
            var testItems = new Faker<LineItem>()
                .CustomInstantiator(f => new LineItem(Usd, Language.InvariantLanguage))
                .RuleFor(i => i.Id, f => f.Random.Guid().ToString())
                .RuleFor(i => i.ListPrice, f => new Money(f.Random.Decimal(1, 50), Usd))
                .RuleFor(i => i.SalePrice, (f, i) => i.ListPrice)
                .RuleFor(i => i.Product, (f, i) => new Product
                {
                    Price = new ProductPrice(Usd)
                    {
                        ListPrice = i.ListPrice                        
                    },
                    IsActive = false
                })
                .RuleFor(i => i.Product, (f, i) => new Product(Usd, Language.InvariantLanguage)
                {
                    IsActive = true,
                    IsBuyable = true,
                    IsAvailable = true,
                    IsInStock = true,
                    Inventory = new inventory.Inventory()
                    {
                        InStockQuantity = InStockQuantity
                    }
                } );

           
            var testShipments = new Faker<Shipment>()
                .CustomInstantiator(f => new Shipment(Usd))
                .RuleFor(s => s.ShipmentMethodCode, f => f.PickRandom(ShipmentMehodCodes))
                .RuleFor(s => s.ShipmentMethodOption, (f, s) => ShippingMethods.FirstOrDefault(x=>x.ShipmentMethodCode == s.ShipmentMethodCode).OptionName)
                .RuleFor(s => s.Price, (f,s) => ShippingMethods.FirstOrDefault(x => x.ShipmentMethodCode == s.ShipmentMethodCode).Price);

            var testCart = new Faker<ShoppingCart>()
                .CustomInstantiator(f => new ShoppingCart(Usd, Language.InvariantLanguage))
                .RuleFor(c => c.Name, f => CART_NAME)
                .RuleFor(c => c.CustomerId, f => Guid.NewGuid().ToString())
                .RuleFor(c => c.CustomerName, f => f.Name.FullName())
                .RuleFor(c => c.Items, f => testItems.Generate(5).ToList())
                .RuleFor(c => c.Shipments, f => testShipments.Generate(1).ToList());

            var cart = testCart.Generate();
            return cart;
        }

        private ShoppingCart GetInvalidCart()
        {
            var testCart = new Faker<ShoppingCart>()
                .CustomInstantiator(f => new ShoppingCart(Usd, Language.InvariantLanguage))
                .RuleFor(c => c.Name, f => null)
                .RuleFor(c => c.CustomerId, f => null);                

            var cart = testCart.Generate();
            return cart;
        }

        private static IEnumerable<ShippingMethod> GetShippingMethods()
        {
            var shippingMethods = new List<ShippingMethod>();

            //testShippingMetjods  = new Faker<ShippingMethod>()
            //    .RuleFor(x=>x.Price)

            var random = new Random();

            foreach(var code in ShipmentMehodCodes)
            {
                var method = new ShippingMethod
                {
                    ShipmentMethodCode = code,
                    OptionName = code,
                    Price = new Money((decimal)random.Next(0, 100), Usd)
                };
                shippingMethods.Add(method);
            }

            return shippingMethods;           
        }
    }
}
