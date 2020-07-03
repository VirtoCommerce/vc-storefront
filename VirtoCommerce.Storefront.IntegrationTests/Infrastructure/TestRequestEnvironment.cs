using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    static class TestRequestEnvironment
    {
        public static Payment PaymentIsRegistered => new Payment(Currency)
        {
            PaymentGatewayCode = "DefaultManualPaymentMethod",
            Amount = new Money(900.00, Currency)
        };

        public static Payment PaymentIsNotRegistered => new Payment(Currency)
        {
            PaymentGatewayCode = "TestPaymentGatewayCode_IsNotRegistered",
            Amount = new Money(900.00, Currency)
        };

        public static Shipment ShipmentIsRegistered => new Shipment(Currency)
        {
            ShipmentMethodCode = "FixedRate",
            ShipmentMethodOption = "Ground"
        };

        public static Shipment ShipmentIsNotRegistered => new Shipment(Currency)
        {
            ShipmentMethodCode = "TestShipmentMethodCode_IsNotRegistered",
            ShipmentMethodOption = "Ground"
        };

        private static Currency Currency => new Currency(Language, "USD");

        private static Language Language => new Language("en-US");
    }
}
