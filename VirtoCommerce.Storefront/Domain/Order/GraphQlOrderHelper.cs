using VirtoCommerce.Storefront.Model.Order.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public static class GraphQlOrderHelper
    {
        public const string AllAddressFields = "city countryCode countryName email firstName key lastName line1 line2 middleName name organization phone postalCode regionId regionName zip addressType";
        public static readonly string AllTaxDetailFields = $"amount{{{AllMoneyFields}}} name rate{{{AllMoneyFields}}}";
        public static readonly string AllDiscountFields = $"coupon amount{{{AllMoneyFields}}} promotionId";
        public const string AllCurrencyFields = "code customFormatting exchangeRate symbol";
        public const string AllShipmentMethodFields = "code id isActive logoUrl priority storeId taxType typeName";
        public const string AllMoneyFields = "amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency";

        public static readonly string AllPaymentMethodFields = $"code currency{{{AllCurrencyFields}}} discountAmount discountAmountWithTax isActive isAvailableForPartial logoUrl" +
            $" name paymentMethodGroupType paymentMethodType price priceWithTax priority storeId taxPercentRate taxTotal taxType total totalWithTax typeName" +
            $" taxDetails {{{AllTaxDetailFields}}}";

        public static readonly string AllPaymentInFields = $"authorizedDate cancelledDate cancelReason capturedDate comment createdBy createdDate " +
            $" customerId customerName gatewayCode id incomingDate isApproved isCancelled modifiedBy modifiedDate number objectType operationType orderId organizationId" +
            $" outerId purpose status sum{{{AllMoneyFields}}} voidedDate tax{{{AllMoneyFields}}}" +
            $" billingAddress {{{AllAddressFields}}} paymentMethod currency {{{AllCurrencyFields}}}";

        public const string AllShipmentPackageFields = "barCode height id items {quantity} length measureUnit packageType weight weightUnit width";
        
        public static readonly string AllLineItemFields = $" cancelledDate cancelReason catalogId categoryId comment currency {{{AllCurrencyFields}}} " +
            $" discountAmount{{{AllMoneyFields}}} discountAmountWithTax{{{AllMoneyFields}}} discountTotal{{{AllMoneyFields}}} discountTotalWithTax{{{AllMoneyFields}}} " +
            $" extendedPrice{{{AllMoneyFields}}} extendedPriceWithTax{{{AllMoneyFields}}} placedPrice{{{AllMoneyFields}}} placedPriceWithTax{{{AllMoneyFields}}} taxTotal{{{AllMoneyFields}}} " +
            $" fee feeWithTax fulfillmentCenterId fulfillmentCenterName fulfillmentLocationCode" +
            $" height id imageUrl isCancelled isGift length measureUnit name objectType outerId price priceId priceWithTax productId productType" +
            $" quantity reserveQuantity shippingMethodCode sku taxDetails {{{AllTaxDetailFields}}} taxPercentRate taxType weight weightUnit width" +
            $" discounts {{{AllDiscountFields}}}";

        public static readonly string AllShipmentItemFields = $"barCode id lineItem {{{AllLineItemFields}}} lineItemId outerId quantity";

        public static readonly string AllShipmentFields = $"customerOrderId discountAmount{{{AllMoneyFields}}} discountAmountWithTax{{{AllMoneyFields}}} employeeId employeeName fee feeWithTax" +
            $" fulfillmentCenterId fulfillmentCenterName height id length measureUnit organizationId organizationName " +
            $" isApproved isCancelled number objectType operationType status" +
            $" price{{{AllMoneyFields}}} priceWithTax{{{AllMoneyFields}}}" +
            $" shipmentMethodCode shipmentMethodOption taxPercentRate taxTotal{{{AllMoneyFields}}} taxType total{{{AllMoneyFields}}} totalWithTax{{{AllMoneyFields}}} weight weightUnit width" +
            $" deliveryAddress {{{AllAddressFields}}} inPayments {{{AllPaymentInFields}}} items {{{AllShipmentItemFields}}} packages {{{AllShipmentPackageFields}}} " +
            $" shippingMethod {{{AllShipmentMethodFields}}} taxDetails {{{AllTaxDetailFields}}} discounts {{{AllDiscountFields}}} currency {{{AllCurrencyFields}}}";


        public static readonly string AllOrderFields = $"id createdDate cancelledDate cancelReason channelId comment  customerId customerName " +
            $" discountAmount{{{AllMoneyFields}}} discountTotal{{{AllMoneyFields}}} discountTotalWithTax{{{AllMoneyFields}}}" +
            $" employeeId employeeName fee feeTotal feeTotalWithTax feeWithTax isApproved isCancelled isPrototype" +
            $" languageCode number objectType operationType organizationId organizationName outerId  shoppingCartId" +
            $" status storeId storeName subscriptionId subscriptionNumber" +
            $" sum taxPercentRate taxTotal{{{AllMoneyFields}}} taxType" +
            $" items {{{AllLineItemFields}}} inPayments {{{AllPaymentInFields}}} shipments {{{AllShipmentFields}}} taxDetails {{{AllTaxDetailFields}}}" +
            $" currency {{{AllCurrencyFields}}} " +
            $" paymentDiscountTotalWithTax{{{AllMoneyFields}}} paymentSubTotal{{{AllMoneyFields}}} paymentSubTotalWithTax{{{AllMoneyFields}}} paymentTaxTotal{{{AllMoneyFields}}} " +
            $" paymentTotal{{{AllMoneyFields}}} paymentTotalWithTax{{{AllMoneyFields}}} shippingDiscountTotal{{{AllMoneyFields}}} shippingDiscountTotalWithTax{{{AllMoneyFields}}}" +
            $" shippingSubTotal{{{AllMoneyFields}}} shippingSubTotalWithTax{{{AllMoneyFields}}} shippingTaxTotal{{{AllMoneyFields}}} shippingTotal{{{AllMoneyFields}}} " +
            $" shippingTotalWithTax{{{AllMoneyFields}}} paymentDiscountTotal{{{AllMoneyFields}}} " +
            $" subTotal{{{AllMoneyFields}}} subTotalDiscount{{{AllMoneyFields}}} subTotalDiscountWithTax{{{AllMoneyFields}}} subTotalTaxTotal{{{AllMoneyFields}}} " +
            $" subTotalWithTax{{{AllMoneyFields}}} total{{{AllMoneyFields}}} addresses {{{AllAddressFields}}} "
            ;

        public static readonly string AllOrderSearchFields = $"totalCount items {{ {AllOrderFields } }}";

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

        public static string SearchOrdersRequest(this ICustomerOrderService service, string sort, string filter, string language, int first = 20, int after = 0, string selectedFields = null)
        => $@"
        {{
            orders(first:{first}, after:""{after}"", sort:""{sort}"", filter:""{filter}"", language:""{language}"")
            {{
             { selectedFields ?? AllOrderSearchFields }
            }}
        }}";

        public static string CreateOrderRequest(this ICustomerOrderService service, string selectedFields = null)
        => $@"mutation ($command: InputCreateOrderFromCartType!){{
          order:createOrderFromCart(command: $command)
            {{
             { selectedFields ?? AllOrderFields }
            }}
        }}";

        public static string UpdateOrderRequest(this ICustomerOrderService service, string selectedFields = null)
        => $@"mutation ($command: InputUpdateOrderType!){{
          updateOrder(command: $command)
        }}";
    }
}
