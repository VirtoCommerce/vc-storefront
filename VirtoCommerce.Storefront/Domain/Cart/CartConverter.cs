using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Tax;
using cartDto = VirtoCommerce.Storefront.AutoRestClients.CartModuleApi.Models;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using marketingDto = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CartConverterExtension
    {
        public static CartConverter CartConverterInstance
        {
            get
            {
                return new CartConverter();
            }
        }

        public static ShoppingCart ToShoppingCart(this cartDto.ShoppingCart cartDto, Currency currency, Language language, User user)
        {
            return CartConverterInstance.ToShoppingCart(cartDto, currency, language, user);
        }

        public static cartDto.ShoppingCart ToShoppingCartDto(this ShoppingCart cart)
        {
            return CartConverterInstance.ToShoppingCartDto(cart);
        }

        public static PromotionEvaluationContext ToPromotionEvaluationContext(this ShoppingCart cart)
        {
            return CartConverterInstance.ToPromotionEvaluationContext(cart);
        }

        public static TaxEvaluationContext ToTaxEvalContext(this ShoppingCart cart, Store store)
        {
            return CartConverterInstance.ToTaxEvalContext(cart, store);
        }

        public static TaxDetail ToTaxDetail(this cartDto.TaxDetail taxDetail, Currency currency)
        {
            return CartConverterInstance.ToTaxDetail(taxDetail, currency);
        }

        public static cartDto.TaxDetail ToCartTaxDetailDto(this TaxDetail taxDetail)
        {
            return CartConverterInstance.ToCartTaxDetailDto(taxDetail);
        }

        public static LineItem ToLineItem(this Product product, Language language, int quantity)
        {
            return CartConverterInstance.ToLineItem(product, language, quantity);
        }

        public static LineItem ToLineItem(this cartDto.LineItem lineItemDto, Currency currency, Language language)
        {
            return CartConverterInstance.ToLineItem(lineItemDto, currency, language);
        }

        public static cartDto.LineItem ToLineItemDto(this LineItem lineItem)
        {
            return CartConverterInstance.ToLineItemDto(lineItem);
        }

        public static CartShipmentItem ToShipmentItem(this LineItem lineItem)
        {
            return CartConverterInstance.ToShipmentItem(lineItem);
        }

        public static marketingDto.ProductPromoEntry ToProductPromoEntryDto(this LineItem lineItem)
        {
            return CartConverterInstance.ToProductPromoEntryDto(lineItem);
        }

        public static cartDto.Address ToCartAddressDto(this Address address)
        {
            return CartConverterInstance.ToCartAddressDto(address);
        }

        public static Address ToAddress(this cartDto.Address addressDto)
        {
            return CartConverterInstance.ToAddress(addressDto);
        }

        public static Payment ToPayment(this cartDto.Payment paymentDto, ShoppingCart cart)
        {
            return CartConverterInstance.ToPayment(paymentDto, cart);
        }

        public static cartDto.Payment ToPaymentDto(this Payment payment)
        {
            return CartConverterInstance.ToPaymentDto(payment);
        }

        public static PaymentMethod ToPaymentMethod(this cartDto.PaymentMethod paymentMethodDto, ShoppingCart cart)
        {
            return CartConverterInstance.ToPaymentMethod(paymentMethodDto, cart);
        }

        public static Payment ToCartPayment(this PaymentMethod paymentMethod, Money amount, ShoppingCart cart)
        {
            return CartConverterInstance.ToCartPayment(paymentMethod, amount, cart);
        }

        public static Shipment ToShipment(this cartDto.Shipment shipmentDto, ShoppingCart cart)
        {
            return CartConverterInstance.ToShipment(shipmentDto, cart);
        }

        public static cartDto.Shipment ToShipmentDto(this Shipment shipment)
        {
            return CartConverterInstance.ToShipmentDto(shipment);
        }

        public static CartShipmentItem ToShipmentItem(this cartDto.ShipmentItem shipmentItemDto, ShoppingCart cart)
        {
            return CartConverterInstance.ToShipmentItem(shipmentItemDto, cart);
        }

        public static cartDto.ShipmentItem ToShipmentItemDto(this CartShipmentItem shipmentItem)
        {
            return CartConverterInstance.ToShipmentItemDto(shipmentItem);
        }

        public static ShippingMethod ToShippingMethod(this cartDto.ShippingRate shippingRate, Currency currency, IEnumerable<Currency> availCurrencies)
        {
            return CartConverterInstance.ToShippingMethod(shippingRate, currency, availCurrencies);

        }
        public static Shipment ToCartShipment(this ShippingMethod shippingMethod, Currency currency)
        {
            return CartConverterInstance.ToCartShipment(shippingMethod, currency);
        }

        public static TaxLine[] ToTaxLines(this ShippingMethod shipmentMethod)
        {
            return CartConverterInstance.ToTaxLines(shipmentMethod);
        }

        public static TaxLine[] ToTaxLines(this PaymentMethod paymentMethod)
        {
            return CartConverterInstance.ToTaxLines(paymentMethod);
        }

        public static Discount ToDiscount(this cartDto.Discount discountDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            return CartConverterInstance.ToDiscount(discountDto, availCurrencies, language);
        }

        public static cartDto.Discount ToCartDiscountDto(this Discount discount)
        {
            return CartConverterInstance.ToCartDiscountDto(discount);
        }

        public static DynamicProperty ToDynamicProperty(this cartDto.DynamicObjectProperty propertyDto)
        {
            return CartConverterInstance.ToDynamicProperty(propertyDto);
        }

        public static cartDto.DynamicObjectProperty ToCartDynamicPropertyDto(this DynamicProperty property)
        {
            return CartConverterInstance.ToCartDynamicPropertyDto(property);
        }
    }

    public partial class CartConverter
    {
        public virtual DynamicProperty ToDynamicProperty(cartDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public virtual cartDto.DynamicObjectProperty ToCartDynamicPropertyDto(DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<cartDto.DynamicObjectProperty>();
        }

        public virtual Discount ToDiscount(cartDto.Discount discountDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(discountDto.Currency)) ?? new Currency(language, discountDto.Currency);

            var result = new Discount(currency);
            result.Coupon = discountDto.Coupon;
            result.Description = discountDto.Description;
            result.PromotionId = discountDto.PromotionId;
            result.Amount = new Money(discountDto.DiscountAmount ?? 0, currency);

            return result;
        }

        public virtual cartDto.Discount ToCartDiscountDto(Discount discount)
        {
            var result = new cartDto.Discount();

            result.PromotionId = discount.PromotionId;
            result.Coupon = discount.Coupon;
            result.Description = discount.Description;
            result.Currency = discount.Amount.Currency.Code;
            result.DiscountAmount = (double)discount.Amount.Amount;
            return result;
        }

        public virtual ShippingMethod ToShippingMethod(cartDto.ShippingRate shippingRate, Currency currency, IEnumerable<Currency> availCurrencies)
        {
            var rateCurrency = availCurrencies.FirstOrDefault(x => x.Equals(shippingRate.Currency)) ?? new Currency(new Language(currency.CultureName), shippingRate.Currency);
            var ratePrice = new Money(shippingRate.Rate ?? 0, rateCurrency);
            var rateDiscount = new Money(shippingRate.DiscountAmount ?? 0, rateCurrency);

            if (rateCurrency != currency)
            {
                ratePrice = ratePrice.ConvertTo(currency);
                rateDiscount = rateDiscount.ConvertTo(currency);
            }

            var result = new ShippingMethod(currency);
            result.OptionDescription = shippingRate.OptionDescription;
            result.OptionName = shippingRate.OptionName;
       
            result.Price = ratePrice;
            result.DiscountAmount = rateDiscount;

            if (shippingRate.ShippingMethod != null)
            {
                result.LogoUrl = shippingRate.ShippingMethod.LogoUrl;
                result.Name = shippingRate.ShippingMethod.Name;
                result.Priority = shippingRate.ShippingMethod.Priority ?? 0;
                result.TaxType = shippingRate.ShippingMethod.TaxType;
               
                result.ShipmentMethodCode = shippingRate.ShippingMethod.Code;
                if (shippingRate.ShippingMethod.Settings != null)
                {
                    result.Settings = shippingRate.ShippingMethod.Settings.Where(x=> !x.ValueType.EqualsInvariant("SecureString"))
                                                                          .Select(x => x.JsonConvert<platformDto.Setting>().ToSettingEntry()).ToList();
                }
            }

            return result;
        }

        public virtual Shipment ToCartShipment(ShippingMethod shippingMethod, Currency currency)
        {
            var result = new Shipment(currency);

            result.ShipmentMethodCode = shippingMethod.ShipmentMethodCode;
            result.Price = shippingMethod.Price;
            result.DiscountAmount = shippingMethod.DiscountAmount;
            result.TaxType = shippingMethod.TaxType;

            return result;
        }

        public virtual TaxLine[] ToTaxLines(ShippingMethod shipmentMethod)
        {
            var retVal = new List<TaxLine>
            {
                new TaxLine(shipmentMethod.Currency)
                {
                    Id = shipmentMethod.BuildTaxLineId(),
                    Code = shipmentMethod.ShipmentMethodCode,
                    TaxType = shipmentMethod.TaxType,
                    //Special case when shipment method have 100% discount and need to calculate tax for old value
                    Amount = shipmentMethod.Total.Amount > 0 ? shipmentMethod.Total : shipmentMethod.Price
                }
            };
            return retVal.ToArray();
        }

        public virtual TaxLine[] ToTaxLines(PaymentMethod paymentMethod)
        {
            var retVal = new List<TaxLine>
            {
                new TaxLine(paymentMethod.Currency)
                {
                    Id = paymentMethod.Code,
                    Code = paymentMethod.Code,
                    TaxType = paymentMethod.TaxType,
                     //Special case when payment method have 100% discount and need to calculate tax for old value
                    Amount = paymentMethod.Total.Amount > 0 ? paymentMethod.Total : paymentMethod.Price
                }
            };
            return retVal.ToArray();
        }

        public virtual CartShipmentItem ToShipmentItem(cartDto.ShipmentItem shipmentItemDto, ShoppingCart cart)
        {
            var result = new CartShipmentItem();

            result.Quantity = shipmentItemDto.Quantity ?? 0;            

            result.LineItem = cart.Items.FirstOrDefault(x => x.Id == shipmentItemDto.LineItemId);

            return result;
        }

        public virtual cartDto.ShipmentItem ToShipmentItemDto(CartShipmentItem shipmentItem)
        {
            var result = new cartDto.ShipmentItem();

            result.Quantity = shipmentItem.Quantity;
            result.LineItem = shipmentItem.LineItem.ToLineItemDto();

            return result;
        }

        public virtual Shipment ToShipment(cartDto.Shipment shipmentDto, ShoppingCart cart)
        {
            var retVal = new Shipment(cart.Currency);

            retVal.Id = shipmentDto.Id;
            retVal.MeasureUnit = shipmentDto.MeasureUnit;
            retVal.ShipmentMethodCode = shipmentDto.ShipmentMethodCode;
            retVal.ShipmentMethodOption = shipmentDto.ShipmentMethodOption;
            retVal.WeightUnit = shipmentDto.WeightUnit;
            retVal.Height = shipmentDto.Height;
            retVal.Weight = shipmentDto.Weight;
            retVal.Width = shipmentDto.Width;
            retVal.Length = shipmentDto.Length;


            retVal.Currency = cart.Currency;
            retVal.Price = new Money(shipmentDto.Price ?? 0, cart.Currency);
            retVal.DiscountAmount = new Money(shipmentDto.DiscountAmount ?? 0, cart.Currency);
            retVal.TaxPercentRate = (decimal?)shipmentDto.TaxPercentRate ?? 0m;

            if (shipmentDto.DeliveryAddress != null)
            {
                retVal.DeliveryAddress = ToAddress(shipmentDto.DeliveryAddress);
            }

            if (shipmentDto.Items != null)
            {
                retVal.Items = shipmentDto.Items.Select(i => ToShipmentItem(i, cart)).ToList();
            }

            if (shipmentDto.TaxDetails != null)
            {
                retVal.TaxDetails = shipmentDto.TaxDetails.Select(td => ToTaxDetail(td, cart.Currency)).ToList();
            }

            if (!shipmentDto.Discounts.IsNullOrEmpty())
            {
                retVal.Discounts.AddRange(shipmentDto.Discounts.Select(x => ToDiscount(x, new[] { cart.Currency }, cart.Language)));
            }
            return retVal;
        }

        public virtual cartDto.Shipment ToShipmentDto(Shipment shipment)
        {
            var retVal = new cartDto.Shipment();

            retVal.Id = shipment.Id;
            retVal.MeasureUnit = shipment.MeasureUnit;
            retVal.ShipmentMethodCode = shipment.ShipmentMethodCode;
            retVal.ShipmentMethodOption = shipment.ShipmentMethodOption;
            retVal.WeightUnit = shipment.WeightUnit;
            retVal.Height = shipment.Height;
            retVal.Weight = shipment.Weight;
            retVal.Width = shipment.Width;
            retVal.Length = shipment.Length;

            retVal.Currency = shipment.Currency != null ? shipment.Currency.Code : null;
            retVal.DiscountAmount = shipment.DiscountAmount != null ? (double?)shipment.DiscountAmount.InternalAmount : null;
            retVal.Price = shipment.Price != null ? (double?)shipment.Price.InternalAmount : null;
            retVal.TaxPercentRate = (double)shipment.TaxPercentRate;

            if (shipment.DeliveryAddress != null)
            {
                retVal.DeliveryAddress = ToCartAddressDto(shipment.DeliveryAddress);
            }

            if (shipment.Discounts != null)
            {
                retVal.Discounts = shipment.Discounts.Select(ToCartDiscountDto).ToList();
            }

            if (shipment.Items != null)
            {
                retVal.Items = shipment.Items.Select(ToShipmentItemDto).ToList();
            }

            if (shipment.TaxDetails != null)
            {
                retVal.TaxDetails = shipment.TaxDetails.Select(ToCartTaxDetailDto).ToList();
            }

            return retVal;
        }

        public virtual PaymentMethod ToPaymentMethod(cartDto.PaymentMethod paymentMethodDto, ShoppingCart cart)
        {
            var retVal = new PaymentMethod(cart.Currency);

            retVal.Code = paymentMethodDto.Code;
            retVal.Description = paymentMethodDto.Description;
            retVal.LogoUrl = paymentMethodDto.LogoUrl;
            retVal.Name = paymentMethodDto.Name;
            retVal.PaymentMethodGroupType = paymentMethodDto.PaymentMethodGroupType;
            retVal.PaymentMethodType = paymentMethodDto.PaymentMethodType;
            retVal.TaxType = paymentMethodDto.TaxType;
            
            retVal.Priority = paymentMethodDto.Priority ?? 0;

            if (paymentMethodDto.Settings != null)
            {
                retVal.Settings = paymentMethodDto.Settings.Where(x => !x.ValueType.EqualsInvariant("SecureString")).Select(x => x.JsonConvert<platformDto.Setting>().ToSettingEntry()).ToList();
            }

            retVal.Currency = cart.Currency;
            retVal.Price = new Money(paymentMethodDto.Price ?? 0, cart.Currency);
            retVal.DiscountAmount = new Money(paymentMethodDto.DiscountAmount ?? 0, cart.Currency);
            retVal.TaxPercentRate = (decimal?)paymentMethodDto.TaxPercentRate ?? 0m;

            if (paymentMethodDto.TaxDetails != null)
            {
                retVal.TaxDetails = paymentMethodDto.TaxDetails.Select(td => ToTaxDetail(td, cart.Currency)).ToList();
            }

            return retVal;
        }

        public virtual Payment ToCartPayment(PaymentMethod paymentMethod, Money amount, ShoppingCart cart)
        {
            var result = new Payment(cart.Currency);

            result.Amount = amount;
            result.PaymentGatewayCode = paymentMethod.Code;
            result.Price = paymentMethod.Price;
            result.DiscountAmount = paymentMethod.DiscountAmount;
            result.TaxPercentRate = paymentMethod.TaxPercentRate;
            result.TaxDetails = paymentMethod.TaxDetails;

            return result;
        }

        public virtual Payment ToPayment(cartDto.Payment paymentDto, ShoppingCart cart)
        {
            var result = new Payment(cart.Currency);

            result.Id = paymentDto.Id;
            result.OuterId = paymentDto.OuterId;
            result.PaymentGatewayCode = paymentDto.PaymentGatewayCode;
            result.TaxType = paymentDto.TaxType;

            result.Amount = new Money(paymentDto.Amount ?? 0, cart.Currency);

            if (paymentDto.BillingAddress != null)
            {
                result.BillingAddress = ToAddress(paymentDto.BillingAddress);
            }

            result.Price = new Money(paymentDto.Price ?? 0, cart.Currency);
            result.DiscountAmount = new Money(paymentDto.DiscountAmount ?? 0, cart.Currency);
            result.TaxPercentRate = (decimal?)paymentDto.TaxPercentRate ?? 0m;

            if (paymentDto.TaxDetails != null)
            {
                result.TaxDetails = paymentDto.TaxDetails.Select(td => ToTaxDetail(td, cart.Currency)).ToList();
            }
            if (!paymentDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(paymentDto.Discounts.Select(x => ToDiscount(x, new[] { cart.Currency }, cart.Language)));
            }
            return result;
        }

        public virtual cartDto.Payment ToPaymentDto(Payment payment)
        {
            var result = new cartDto.Payment();

            result.Id = payment.Id;
            result.OuterId = payment.OuterId;
            result.PaymentGatewayCode = payment.PaymentGatewayCode;
            result.TaxType = payment.TaxType;

            result.Amount = (double)payment.Amount.InternalAmount;

            result.Currency = payment.Currency.Code;
            result.Price = (double)payment.Price.InternalAmount;
            result.DiscountAmount = (double)payment.DiscountAmount.InternalAmount;
            result.TaxPercentRate = (double)payment.TaxPercentRate;

            if (payment.BillingAddress != null)
            {
                result.BillingAddress = ToCartAddressDto(payment.BillingAddress);
            }
            if (payment.Discounts != null)
            {
                result.Discounts = payment.Discounts.Select(ToCartDiscountDto).ToList();
            }
            if (payment.TaxDetails != null)
            {
                result.TaxDetails = payment.TaxDetails.Select(ToCartTaxDetailDto).ToList();
            }

            return result;
        }

        public virtual cartDto.Address ToCartAddressDto(Address address)
        {
            return address.ToCoreAddressDto().JsonConvert<cartDto.Address>();
        }

        public virtual Address ToAddress(cartDto.Address addressDto)
        {
            return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public virtual PromotionEvaluationContext ToPromotionEvaluationContext(ShoppingCart cart)
        {
            var result = new PromotionEvaluationContext(cart.Language, cart.Currency)
            {
                Cart = cart,
                User = cart.Customer,
                Currency = cart.Currency,
                Language = cart.Language,
                StoreId = cart.StoreId
            };

            return result;
        }

        public virtual ShoppingCart ToShoppingCart(cartDto.ShoppingCart cartDto, Currency currency, Language language, User user)
        {
            var result = new ShoppingCart(currency, language);

            result.ChannelId = cartDto.ChannelId;
            result.Comment = cartDto.Comment;
            result.CustomerId = cartDto.CustomerId;
            result.CustomerName = cartDto.CustomerName;
            result.Id = cartDto.Id;
            result.Name = cartDto.Name;
            result.ObjectType = cartDto.ObjectType;
            result.OrganizationId = cartDto.OrganizationId;
            result.Status = cartDto.Status;
            result.StoreId = cartDto.StoreId;
       
            result.Customer = user;

            if (cartDto.Coupon != null)
            {
                result.Coupon = new Coupon
                {
                    Code = cartDto.Coupon,
                    AppliedSuccessfully = !string.IsNullOrEmpty(cartDto.Coupon)
                };
            }

            if (cartDto.Items != null)
            {
                result.Items = cartDto.Items.Select(i => ToLineItem(i, currency, language)).ToList();
                result.HasPhysicalProducts = result.Items.Any(i =>
                    string.IsNullOrEmpty(i.ProductType) ||
                    !string.IsNullOrEmpty(i.ProductType) && i.ProductType.Equals("Physical", StringComparison.OrdinalIgnoreCase));
            }

            if (cartDto.Addresses != null)
            {
                result.Addresses = cartDto.Addresses.Select(ToAddress).ToList();
            }

            if (cartDto.Payments != null)
            {
                result.Payments = cartDto.Payments.Select(p => ToPayment(p, result)).ToList();
            }

            if (cartDto.Shipments != null)
            {
                result.Shipments = cartDto.Shipments.Select(s => ToShipment(s, result)).ToList();
            }

            if (cartDto.DynamicProperties != null)
            {
                result.DynamicProperties = cartDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            if (cartDto.TaxDetails != null)
            {
                result.TaxDetails = cartDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            result.DiscountAmount = new Money(cartDto.DiscountAmount ?? 0, currency);
            result.HandlingTotal = new Money(cartDto.HandlingTotal ?? 0, currency);
            result.HandlingTotalWithTax = new Money(cartDto.HandlingTotalWithTax ?? 0, currency);
            result.IsAnonymous = cartDto.IsAnonymous == true;
            result.IsRecuring = cartDto.IsRecuring == true;
            result.VolumetricWeight = (decimal)(cartDto.VolumetricWeight ?? 0);
            result.Weight = (decimal)(cartDto.Weight ?? 0);

            return result;
        }


        public virtual cartDto.ShoppingCart ToShoppingCartDto(ShoppingCart cart)
        {
            var result = new cartDto.ShoppingCart();

            result.ChannelId = cart.ChannelId;
            result.Comment = cart.Comment;
            result.CustomerId = cart.CustomerId;
            result.CustomerName = cart.CustomerName;
            result.Id = cart.Id;
            result.Name = cart.Name;
            result.ObjectType = cart.ObjectType;
            result.OrganizationId = cart.OrganizationId;
            result.Status = cart.Status;
            result.StoreId = cart.StoreId;

            if (cart.Language != null)
            {
                result.LanguageCode = cart.Language.CultureName;
            }
            result.Addresses = cart.Addresses.Select(ToCartAddressDto).ToList();
            result.Coupon = cart.Coupon != null ? cart.Coupon.Code : null;
            result.Currency = cart.Currency.Code;
            result.Discounts = cart.Discounts.Select(ToCartDiscountDto).ToList();
            result.HandlingTotal = (double)cart.HandlingTotal.InternalAmount;
            result.HandlingTotalWithTax = (double)cart.HandlingTotal.InternalAmount;
            result.DiscountAmount = (double)cart.DiscountAmount.InternalAmount;
            result.Items = cart.Items.Select(ToLineItemDto).ToList();
            result.Payments = cart.Payments.Select(ToPaymentDto).ToList();
            result.Shipments = cart.Shipments.Select(ToShipmentDto).ToList();
            result.TaxDetails = cart.TaxDetails.Select(ToCartTaxDetailDto).ToList();
            result.DynamicProperties = cart.DynamicProperties.Select(ToCartDynamicPropertyDto).ToList();
            result.VolumetricWeight = (double)cart.VolumetricWeight;
            result.Weight = (double)cart.Weight;

            return result;
        }

        public virtual TaxEvaluationContext ToTaxEvalContext(ShoppingCart cart, Store store)
        {
            var result = new TaxEvaluationContext(cart.StoreId);

            result.Id = cart.Id;
            result.Code = cart.Name;
            result.Currency = cart.Currency;
            result.Type = "Cart";
            result.Customer = cart.Customer;
            result.StoreTaxCalculationEnabled = store.TaxCalculationEnabled;

            foreach (var lineItem in cart.Items)
            {
                result.Lines.Add(new TaxLine(lineItem.Currency)
                {
                    Id = lineItem.Id,
                    Code = lineItem.Sku,
                    Name = lineItem.Name,
                    TaxType = lineItem.TaxType,
                    //Special case when product have 100% discount and need to calculate tax for old value
                    Amount = lineItem.ExtendedPrice.Amount > 0 ? lineItem.ExtendedPrice : lineItem.SalePrice
                });
            }

            foreach (var shipment in cart.Shipments)
            {
                var totalTaxLine = new TaxLine(shipment.Currency)
                {
                    Id = shipment.Id,
                    Code = shipment.ShipmentMethodCode,
                    Name = shipment.ShipmentMethodOption,
                    TaxType = shipment.TaxType,
                    //Special case when shipment have 100% discount and need to calculate tax for old value
                    Amount = shipment.Total.Amount > 0 ? shipment.Total : shipment.Price
                };
                result.Lines.Add(totalTaxLine);

                if (shipment.DeliveryAddress != null)
                {
                    result.Address = shipment.DeliveryAddress;
                }
            }

            foreach (var payment in cart.Payments)
            {
                var totalTaxLine = new TaxLine(payment.Currency)
                {
                    Id = payment.Id,
                    Code = payment.PaymentGatewayCode,
                    Name = payment.PaymentGatewayCode,
                    TaxType = payment.TaxType,
                    //Special case when shipment have 100% discount and need to calculate tax for old value
                    Amount = payment.Total.Amount > 0 ? payment.Total : payment.Price
                };
                result.Lines.Add(totalTaxLine);
            }
            return result;
        }

        public virtual TaxDetail ToTaxDetail(cartDto.TaxDetail taxDeatilDto, Currency currency)
        {
            var result = new TaxDetail(currency);
            result.Name = taxDeatilDto.Name;
            result.Rate = new Money(taxDeatilDto.Rate ?? 0, currency);
            return result;
        }

        public virtual cartDto.TaxDetail ToCartTaxDetailDto(TaxDetail taxDetail)
        {
            var result = new cartDto.TaxDetail();
            result.Name = taxDetail.Name;
            result.Rate = (double)taxDetail.Rate.Amount;
            return result;
        }

        public virtual LineItem ToLineItem(Product product, Language language, int quantity)
        {
            var result = new LineItem(product.Price.Currency, language);

            result.CatalogId = product.CatalogId;
            result.CategoryId = product.CategoryId;
            result.Name = product.Name;
            result.Sku = product.Sku;
            result.ProductType = product.ProductType;
            result.TaxType = product.TaxType;
            result.WeightUnit = product.WeightUnit;
            result.MeasureUnit = product.MeasureUnit;
            result.Weight = product.Weight;
            result.Width = product.Width;
            result.Length = product.Length;
            result.Height = product.Height;

            result.ImageUrl = product.PrimaryImage?.Url;
            result.ThumbnailImageUrl = product.PrimaryImage?.Url;
            result.ListPrice = product.Price.ListPrice;
            result.SalePrice = product.Price.GetTierPrice(quantity).Price;
            result.TaxPercentRate = product.Price.TaxPercentRate;
            result.DiscountAmount = product.Price.DiscountAmount;
            result.ProductId = product.Id;
            result.Quantity = quantity;
            result.IsReccuring = result.PaymentPlan != null;

            return result;
        }

        public virtual LineItem ToLineItem(cartDto.LineItem lineItemDto, Currency currency, Language language)
        {
            var result = new LineItem(currency, language);

            result.Id = lineItemDto.Id;
            result.CatalogId = lineItemDto.CatalogId;
            result.CategoryId = lineItemDto.CategoryId;
            result.ImageUrl = lineItemDto.ImageUrl;
            result.Name = lineItemDto.Name;
            result.ObjectType = lineItemDto.ObjectType;
            result.ProductId = lineItemDto.ProductId;
            result.ProductType = lineItemDto.ProductType;
            result.Quantity = lineItemDto.Quantity ?? 1;
            result.ShipmentMethodCode = lineItemDto.ShipmentMethodCode;
            result.Sku = lineItemDto.Sku;
            result.TaxType = lineItemDto.TaxType;
            result.ThumbnailImageUrl = lineItemDto.ThumbnailImageUrl;
            result.WeightUnit = lineItemDto.WeightUnit;
            result.MeasureUnit = lineItemDto.MeasureUnit;
            result.Weight = (decimal?)lineItemDto.Weight;
            result.Width = (decimal?)lineItemDto.Width;
            result.Length = (decimal?)lineItemDto.Length;
            result.Height = (decimal?)lineItemDto.Height;



            result.ImageUrl = lineItemDto.ImageUrl.RemoveLeadingUriScheme();

            if (lineItemDto.TaxDetails != null)
            {
                result.TaxDetails = lineItemDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            if (lineItemDto.DynamicProperties != null)
            {
                result.DynamicProperties = lineItemDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            if (!lineItemDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(lineItemDto.Discounts.Select(x => ToDiscount(x, new[] { currency }, language)));
            }
            result.Comment = lineItemDto.Note;
            result.IsGift = lineItemDto.IsGift == true;
            result.IsReccuring = lineItemDto.IsReccuring == true;
            result.ListPrice = new Money(lineItemDto.ListPrice ?? 0, currency);
            result.RequiredShipping = lineItemDto.RequiredShipping == true;
            result.SalePrice = new Money(lineItemDto.SalePrice ?? 0, currency);
            result.TaxPercentRate = (decimal?)lineItemDto.TaxPercentRate ?? 0m;
            result.DiscountAmount = new Money(lineItemDto.DiscountAmount ?? 0, currency);
            result.TaxIncluded = lineItemDto.TaxIncluded == true;
            result.Weight = (decimal?)lineItemDto.Weight;
            result.Width = (decimal?)lineItemDto.Width;
            result.Height = (decimal?)lineItemDto.Height;
            result.Length = (decimal?)lineItemDto.Length;

            return result;
        }

        public virtual cartDto.LineItem ToLineItemDto(LineItem lineItem)
        {
            var retVal = new cartDto.LineItem();

            retVal.Id = lineItem.Id;
            retVal.CatalogId = lineItem.CatalogId;
            retVal.CategoryId = lineItem.CategoryId;
            retVal.ImageUrl = lineItem.ImageUrl;
            retVal.Name = lineItem.Name;
            retVal.ObjectType = lineItem.ObjectType;
            retVal.ProductId = lineItem.ProductId;
            retVal.ProductType = lineItem.ProductType;
            retVal.Quantity = lineItem.Quantity;
            retVal.ShipmentMethodCode = lineItem.ShipmentMethodCode;
            retVal.Sku = lineItem.Sku;
            retVal.TaxType = lineItem.TaxType;
            retVal.ThumbnailImageUrl = lineItem.ThumbnailImageUrl;
            retVal.WeightUnit = lineItem.WeightUnit;
            retVal.MeasureUnit = lineItem.MeasureUnit;
            retVal.Weight = (double?)lineItem.Weight;
            retVal.Width = (double?)lineItem.Width;
            retVal.Length = (double?)lineItem.Length;
            retVal.Height = (double?)lineItem.Height;

            retVal.Currency = lineItem.Currency.Code;
            retVal.Discounts = lineItem.Discounts.Select(ToCartDiscountDto).ToList();

            retVal.ListPrice = (double)lineItem.ListPrice.InternalAmount;
            retVal.SalePrice = (double)lineItem.SalePrice.InternalAmount;
            retVal.TaxPercentRate = (double)lineItem.TaxPercentRate;
            retVal.DiscountAmount = (double)lineItem.DiscountAmount.InternalAmount;
            retVal.TaxDetails = lineItem.TaxDetails.Select(ToCartTaxDetailDto).ToList();
            retVal.DynamicProperties = lineItem.DynamicProperties.Select(ToCartDynamicPropertyDto).ToList();
            retVal.VolumetricWeight = (double)(lineItem.VolumetricWeight ?? 0);
            retVal.Weight = (double?)lineItem.Weight;
            retVal.Width = (double?)lineItem.Width;
            retVal.Height = (double?)lineItem.Height;
            retVal.Length = (double?)lineItem.Length;

            return retVal;
        }

        public virtual CartShipmentItem ToShipmentItem(LineItem lineItem)
        {
            var shipmentItem = new CartShipmentItem
            {
                LineItem = lineItem,
                Quantity = lineItem.Quantity
            };
            return shipmentItem;
        }

        public virtual marketingDto.ProductPromoEntry ToProductPromoEntryDto(LineItem lineItem)
        {
            var result = new marketingDto.ProductPromoEntry();

            result.CatalogId = lineItem.CatalogId;
            result.CategoryId = lineItem.CategoryId;
            result.Code = lineItem.Sku;
            result.ProductId = lineItem.ProductId;
            result.Discount = (double)lineItem.DiscountTotal.Amount;
            //Use only base price for discount evaluation
            result.Price = (double)lineItem.SalePrice.Amount;
            result.Quantity = lineItem.Quantity;
            result.InStockQuantity = lineItem.InStockQuantity;
            result.Variations = null; // TODO

            return result;
        }
    }
}
