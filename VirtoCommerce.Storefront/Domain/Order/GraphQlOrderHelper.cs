using VirtoCommerce.Storefront.Model.Order.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class GraphQlOrderHelper
    {
        public const string AllAddressFields = "city countryCode countryName email firstName key lastName line1 line2 middleName name organization phone postalCode regionId regionName zip addressType";
        public const string AllTaxDetailFields = "amount name rate";
        public const string AllDiscountFields = "coupon currency description discountAmount discountAmountWithTax promotionId";
        public const string AllCurrencyFields = "code customFormatting exchangeRate isPrimary name symbol";
        public const string AllShipmentMethodFields = "code id isActive logoUrl priority storeId taxType typeName";

        public static readonly string AllPaymentMethodFields = $"code currency discountAmount discountAmountWithTax isActive isAvailableForPartial logoUrl" +
            $" name paymentMethodGroupType paymentMethodType price priceWithTax priority storeId taxPercentRate taxTotal taxType total totalWithTax typeName" +
            $" taxDetails {{{AllTaxDetailFields}}}";

        public static readonly string AllPaymentInFields = $"authorizedDate cancelledDate cancelReason capturedDate comment createdBy createdDate " +
            $" customerId customerName gatewayCode id incomingDate isApproved isCancelled modifiedBy modifiedDate number objectType operationType orderId organizationId" +
            $" outerId purpose status sum voidedDate" +
            $" billingAddress {{{AllAddressFields}}} paymentMethod {{{AllPaymentMethodFields}}} currency {{{AllCurrencyFields}}}";

        public const string AllShipmentPackageFields = "barCode height id items {quantity} length measureUnit packageType weight weightUnit width";
        


        public static readonly string AllLineItemFields = $" cancelledDate cancelReason catalogId categoryId comment currency discountAmount discountAmountWithTax" +
            $" discountTotal discountTotalWithTax extendedPrice extendedPriceWithTax fee feeWithTax fulfillmentCenterId fulfillmentCenterName fulfillmentLocationCode" +
            $" height id imageUrl isCancelled isGift length measureUnit name objectType outerId placedPrice placedPriceWithTax price priceId priceWithTax productId productType" +
            $" quantity reserveQuantity shippingMethodCode sku taxDetails {{{AllTaxDetailFields}}} taxPercentRate taxTotal taxType weight weightUnit width" +
            $" discounts {{{AllDiscountFields}}}";

        public static readonly string AllShipmentItemFields = $"barCode id lineItem {{{AllLineItemFields}}} lineItemId outerId quantity";

        public static readonly string AllShipmentFields = $"customerOrderId discountAmount discountAmountWithTax employeeId employeeName fee feeWithTax" +
            $" fulfillmentCenterId fulfillmentCenterName height id length measureUnit objectType organizationId organizationName price priceWithTax" +
            $" shipmentMethodCode shipmentMethodOption taxPercentRate taxTotal taxType total totalWithTax weight weightUnit width" +
            $" deliveryAddress {{{AllAddressFields}}} inPayments {{{AllPaymentInFields}}} items {{{AllShipmentItemFields}}} packages {{{AllShipmentPackageFields}}} " +
            $" shippingMethod {{{AllShipmentMethodFields}}} taxDetails {{{AllTaxDetailFields}}} discounts {{{AllDiscountFields}}} ";


        public static readonly string AllOrderFields = $"addresses {{{AllAddressFields}}} cancelledDate cancelReason channelId comment currency customerId customerName discountAmount" +
            $" discountTotal discountTotalWithTax employeeId employeeName fee feeTotal feeTotalWithTax feeWithTax id  isApproved isCancelled isPrototype" +
            $" languageCode number objectType operationType organizationId organizationName outerId paymentDiscountTotal" +
            $" paymentDiscountTotalWithTax paymentSubTotal paymentSubTotalWithTax paymentTaxTotal paymentTotal paymentTotalWithTax shippingDiscountTotal" +
            $" shippingDiscountTotalWithTax shippingSubTotal shippingSubTotalWithTax shippingTaxTotal shippingTotal shippingTotalWithTax shoppingCartId" +
            $" status storeId storeName subscriptionId subscriptionNumber subTotal subTotalDiscount subTotalDiscountWithTax subTotalTaxTotal subTotalWithTax" +
            $" sum taxPercentRate taxTotal taxType total" +
            $" items {{{AllLineItemFields}}} inPayments {{{AllPaymentInFields}}} shipments {{{AllShipmentFields}}} taxDetails {{{AllTaxDetailFields}}}"
            ;

        public static string GetOrderByIdRequest(this ICustomerOrderService service, string id, string selectedFields = null)
        => $@"
        {{
            order(id:""{id}"")
            {{
            { selectedFields ?? AllOrderFields }
            }}
        }}";

        public static string GetOrderByNumberRequest(this ICustomerOrderService service, string number, string selectedFields = null)
        => $@"
        {{
            order(number:""{number}"")
            {{
            { selectedFields ?? AllOrderFields }
            }}
        }}";
    }
}
