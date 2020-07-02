namespace VirtoCommerce.Storefront.Infrastructure
{
    public class QueryHelper
    {
        private readonly string externalHeader;
        private readonly string internalHeader;

        public static string GetCart(string storeId, string cartName, string userId, string cultureName, string currencyCode, string type, string selectedFields = null)
        => $@"
        {{
            cart(storeId:""{storeId}"",cartName:""{cartName}"",userId:""{userId}"",cultureName:""{cultureName}"",currencyCode:""{currencyCode}"",type:""{type}"")
            {{
            {selectedFields ?? AllFields()}
            }}
        }}";

        public static string ClearCart(string selectedFields = null)
        => $@"mutation ($command:InputClearCartType!)
        {{
            clearCart(command: $command)
            {{
            {selectedFields ?? AllFields()}
            }}
        }}";

        public static string AddCoupon(string selectedFields = null)
        => $@"mutation ($command:InputAddCouponType!)
        {{
            removeCartItem(command: $command)
            {{
            { selectedFields ?? AllFields() }
            }}
        }}";

        public static string AddItemToCart(string selectedFields = null)
        => $@"mutation ($command:InputAddItemType!)
        {{ 
          addItem(command: $command) 
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        public static string AddOrUpdatePayment(string selectedFields = null)
        => $@"mutation ($command:InputAddOrUpdateCartPaymentType!)
        {{ 
          addOrUpdateCartPayment(command: $command)
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        public static string AddOrUpdateShippment(string selectedFields = null)
        => $@"mutation ($command:InputAddOrUpdateCartShipmentType!)
        {{ 
          addOrUpdateCartShipment(command: $command)
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        public static string ChangeCartComment(string selectedFields = null)
        => $@"mutation ($command:InputChangeCommentType!)
        {{ 
          changeComment(command: $command)
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        public static string ChangeCartItemPrice(string selectedFields = null)
        => $@"mutation ($command:InputChangeCartItemPriceType!)
        {{ 
          changeCartItemPrice(command: $command)
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        public static string ChangeCartItemQuantity(string selectedFields = null)
        => $@"mutation ($command:InputChangeCartItemQuantityType!)
        {{ 
          changeCartItemQuantity(command: $command)
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        public static string RemoveCartItem(string selectedFields = null)
        => $@"mutation ($command:InputRemoveItemType!)
        {{ 
          removeCartItem(command: $command)
          {{ 
            { selectedFields ?? AllFields() }
          }}
        }}";

        private static string AllFields()
            => @"
            id 
            name 
            status 
            storeId 
            channelId 
            isAnonymous 
            customerId 
            customerName 
            organizationId 
            isRecuring 
            comment 
            volumetricWeight 
            weightUnit 
            weight 
            currency {code symbol exchangeRate customFormatting} 
            taxPercentRate 
            taxType 
            taxDetails 
            {
              rate {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              amount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              name 
              price {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            } 
            shipments
            {
              shipmentMethodCode 
              shipmentMethodOption 
              fulfillmentCenterId 
              deliveryAddress{key name organization countryCode countryName city postalCode zip line1 line2 regionId regionName firstName middleName lastName phone email} 
              volumetricWeight 
              weightUnit 
              weight 
              measureUnit 
              height 
              length 
              width
            } 
            availableShippingMethods
            {
              code 
              logoUrl 
              optionName 
              optionDescription 
              priority 
              currency{code symbol exchangeRate customFormatting} 
              price {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              priceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              total {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              totalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              discountAmount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              discountAmountWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            } 
            discounts
            {
              promotionId
              amount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              coupon 
              description
            } 
            currency{code symbol exchangeRate customFormatting} 
            payments
            {
              outerId 
              paymentGatewayCode 
              currency{code symbol exchangeRate customFormatting} 
              billingAddress{key name organization countryCode countryName city postalCode zip line1 line2 regionId regionName firstName middleName lastName phone email} 
              taxPercentRate 
              taxType 
              taxDetails
              {
                rate {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
                amount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
                name
                price {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              }
              discounts{promotionId coupon description}
            } 
            availablePaymentMethods
            {
              code 
              name 
              logoUrl 
              paymentMethodType 
              paymentMethodGroupType 
              priority 
              isAvailableForPartial 
              currency {code symbol exchangeRate customFormatting} 
              price {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              priceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              total {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              totalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
              discountAmount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency} 
            	discountAmountWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            	taxTotal{amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              taxPercentRate
              taxType
              taxDetails
              {
                rate {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
                amount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
                name
                price {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              }
            } 
            addresses
            {
              key 
              name 
              organization 
              countryCode 
              countryName 
              city 
              postalCode 
              zip 
              line1 
              line2 
              regionId 
              regionName 
              firstName
              middleName 
              lastName 
              phone 
              email
            } 
            items
            {
              id
              createdDate 
              productId 
              productType 
              catalogId 
              categoryId 
              sku 
              name 
              quantity 
              shipmentMethodCode 
              requiredShipping 
              thumbnailImageUrl 
              imageUrl 
              isGift 
              languageCode 
              note 
              isReccuring 
              volumetricWeight 
              weightUnit 
              weight 
              measureUnit 
              height 
              length 
              width 
              isReadOnly 
              listPrice {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              listPriceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              salePrice {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              salePriceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              placedPrice {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              placedPriceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              extendedPrice {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              extendedPriceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              discountAmount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              discountAmountWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              discountTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              discountTotalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              objectType 
              taxTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
        			taxPercentRate 
              taxType 
               taxDetails
              {
                rate {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
                amount {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
                name
                price {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
              }
            }  
            itemsCount
            itemsQuantity
            coupons {code isAppliedSuccessfully} 
            isValid 
            validationErrors{errorCode} 
            type
            handlingTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            handlingTotalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            total {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            subTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            subTotalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            shippingPrice {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            shippingPriceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            shippingTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            shippingTotalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            paymentPrice {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            paymentPriceWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            paymentTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            paymentTotalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            discountTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            discountTotalWithTax {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}
            taxTotal {amount decimalDigits formattedAmount formattedAmountWithoutPoint formattedAmountWithoutCurrency formattedAmountWithoutPointAndCurrency}";
        //public Mutation(string name, params KeyValuePair<string, string>[] parameters)
        //{
        //    externalHeader = $"mutation ({string.Join(',', parameters.Select(p => $"${p.Key}:{p.Value}").ToList())})";
        //    internalHeader = $"{name}({string.Join(',', parameters.Select(p => $"{p.Key}:${p.Key}").ToList())})";
        //}

        //public Mutation<T> AddField<Prop>(Expression<Func<T, Prop>> selector)
        //{
        //    var body = selector.Body as MemberExpression;
        //    var propertyInfo = (PropertyInfo)body.Member;

        //    var propertyType = propertyInfo.PropertyType;
        //    var propertyName = propertyInfo.Name;
        //}
    }
}
