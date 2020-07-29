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

    public static partial class CartConverter
    {
        public static cartDto.ShoppingCartSearchCriteria ToSearchCriteriaDto(this CartSearchCriteria criteria)
        {
            var result = new cartDto.ShoppingCartSearchCriteria
            {
                Name = criteria.Name,
                Type = criteria.Type,
                StoreId = criteria.StoreId,
                CustomerId = criteria.Customer?.Id,
                Currency = criteria.Currency?.Code,
                LanguageCode = criteria.Language?.CultureName,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };
            return result;
        }

        public static DynamicProperty ToDynamicProperty(this cartDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public static cartDto.DynamicObjectProperty ToCartDynamicPropertyDto(this DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<cartDto.DynamicObjectProperty>();
        }

        public static Discount ToDiscount(this cartDto.Discount discountDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(discountDto.Currency)) ?? new Currency(language, discountDto.Currency);

            var result = new Discount(currency)
            {
                Coupon = discountDto.Coupon,
                Description = discountDto.Description,
                PromotionId = discountDto.PromotionId,
                Amount = new Money(discountDto.DiscountAmount ?? 0, currency)
            };

            return result;
        }

        public static cartDto.Discount ToCartDiscountDto(this Discount discount)
        {
            var result = new cartDto.Discount
            {
                PromotionId = discount.PromotionId,
                Coupon = discount.Coupon,
                Description = discount.Description,
                Currency = discount.Amount.Currency.Code,
                DiscountAmount = (double)discount.Amount.Amount
            };
            return result;
        }

        public static ShippingMethod ToShippingMethod(this cartDto.ShippingRate shippingRate, Currency currency, IEnumerable<Currency> availCurrencies)
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
                    result.Settings = shippingRate.ShippingMethod.Settings.Where(x => !x.ValueType.EqualsInvariant("SecureString"))
                                                                          .Select(x => x.JsonConvert<platformDto.Setting>().ToSettingEntry()).ToList();
                }
            }

            return result;
        }

        public static Shipment ToCartShipment(this ShippingMethod shippingMethod, Currency currency)
        {
            var result = new Shipment(currency)
            {
                ShipmentMethodCode = shippingMethod.ShipmentMethodCode,
                Price = shippingMethod.Price,
                DiscountAmount = shippingMethod.DiscountAmount,
                TaxType = shippingMethod.TaxType
            };

            return result;
        }

        public static TaxLine[] ToTaxLines(this ShippingMethod shipmentMethod)
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

        public static TaxLine[] ToTaxLines(this PaymentMethod paymentMethod)
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

        public static CartShipmentItem ToShipmentItem(this cartDto.ShipmentItem shipmentItemDto, ShoppingCart cart)
        {
            var result = new CartShipmentItem
            {
                Id = shipmentItemDto.Id,
                Quantity = shipmentItemDto.Quantity ?? 0,
                LineItem = cart.Items.FirstOrDefault(x => x.Id == shipmentItemDto.LineItemId)
            };

            return result;
        }

        public static cartDto.ShipmentItem ToShipmentItemDto(this CartShipmentItem shipmentItem)
        {
            var result = new cartDto.ShipmentItem
            {
                Id = shipmentItem.Id,
                Quantity = shipmentItem.Quantity,
                LineItemId = shipmentItem.LineItem.Id,
                LineItem = shipmentItem.LineItem.ToLineItemDto()
            };

            return result;
        }

        public static Shipment ToShipment(this cartDto.Shipment shipmentDto, ShoppingCart cart)
        {
            var retVal = new Shipment(cart.Currency)
            {
                Id = shipmentDto.Id,
                MeasureUnit = shipmentDto.MeasureUnit,
                ShipmentMethodCode = shipmentDto.ShipmentMethodCode,
                ShipmentMethodOption = shipmentDto.ShipmentMethodOption,
                WeightUnit = shipmentDto.WeightUnit,
                Height = shipmentDto.Height,
                Weight = shipmentDto.Weight,
                Width = shipmentDto.Width,
                Length = shipmentDto.Length,
                Currency = cart.Currency,
                Price = new Money(shipmentDto.Price ?? 0, cart.Currency),
                PriceWithTax = new Money(shipmentDto.PriceWithTax ?? 0, cart.Currency),
                DiscountAmount = new Money(shipmentDto.DiscountAmount ?? 0, cart.Currency),
                Total = new Money(shipmentDto.Total ?? 0, cart.Currency),
                TotalWithTax = new Money(shipmentDto.TotalWithTax ?? 0, cart.Currency),
                DiscountAmountWithTax = new Money(shipmentDto.DiscountAmountWithTax ?? 0, cart.Currency),
                TaxTotal = new Money(shipmentDto.TaxTotal ?? 0, cart.Currency),
                TaxPercentRate = (decimal?)shipmentDto.TaxPercentRate ?? 0m,
                TaxType = shipmentDto.TaxType
            };

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

        public static cartDto.Shipment ToShipmentDto(this Shipment shipment)
        {
            var retVal = new cartDto.Shipment
            {
                Id = shipment.Id,
                MeasureUnit = shipment.MeasureUnit,
                ShipmentMethodCode = shipment.ShipmentMethodCode,
                ShipmentMethodOption = shipment.ShipmentMethodOption,
                WeightUnit = shipment.WeightUnit,
                Height = shipment.Height,
                Weight = shipment.Weight,
                Width = shipment.Width,
                Length = shipment.Length,

                Currency = shipment.Currency != null ? shipment.Currency.Code : null,
                DiscountAmount = shipment.DiscountAmount != null ? (double?)shipment.DiscountAmount.InternalAmount : null,
                Price = shipment.Price != null ? (double?)shipment.Price.InternalAmount : null,
                TaxPercentRate = (double)shipment.TaxPercentRate,
                TaxType = shipment.TaxType
            };

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

        public static PaymentMethod ToCartPaymentMethod(this cartDto.PaymentMethod paymentMethodDto, ShoppingCart cart)
        {
            var retVal = new PaymentMethod(cart.Currency)
            {
                Code = paymentMethodDto.Code,
                Description = paymentMethodDto.Description,
                LogoUrl = paymentMethodDto.LogoUrl,
                Name = paymentMethodDto.Name,
                PaymentMethodGroupType = paymentMethodDto.PaymentMethodGroupType,
                PaymentMethodType = paymentMethodDto.PaymentMethodType,
                TaxType = paymentMethodDto.TaxType,

                Priority = paymentMethodDto.Priority ?? 0
            };

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

        public static Payment ToCartPayment(this PaymentMethod paymentMethod, Money amount, ShoppingCart cart)
        {
            var result = new Payment(cart.Currency)
            {
                Amount = amount,
                PaymentGatewayCode = paymentMethod.Code,
                Price = paymentMethod.Price,
                DiscountAmount = paymentMethod.DiscountAmount,
                TaxPercentRate = paymentMethod.TaxPercentRate,
                TaxDetails = paymentMethod.TaxDetails
            };

            return result;
        }

        public static Payment ToPayment(this cartDto.Payment paymentDto, ShoppingCart cart)
        {
            var result = new Payment(cart.Currency)
            {
                Id = paymentDto.Id,
                OuterId = paymentDto.OuterId,
                PaymentGatewayCode = paymentDto.PaymentGatewayCode,
                TaxType = paymentDto.TaxType,

                Amount = new Money(paymentDto.Amount ?? 0, cart.Currency)
            };

            if (paymentDto.BillingAddress != null)
            {
                result.BillingAddress = ToAddress(paymentDto.BillingAddress);
            }

            result.Price = new Money(paymentDto.Price ?? 0, cart.Currency);
            result.DiscountAmount = new Money(paymentDto.DiscountAmount ?? 0, cart.Currency);
            result.PriceWithTax = new Money(paymentDto.PriceWithTax ?? 0, cart.Currency);
            result.DiscountAmountWithTax = new Money(paymentDto.DiscountAmountWithTax ?? 0, cart.Currency);
            result.Total = new Money(paymentDto.Total ?? 0, cart.Currency);
            result.TotalWithTax = new Money(paymentDto.TotalWithTax ?? 0, cart.Currency);
            result.TaxTotal = new Money(paymentDto.TaxTotal ?? 0, cart.Currency);
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

        public static cartDto.Payment ToPaymentDto(this Payment payment)
        {
            var result = new cartDto.Payment
            {
                Id = payment.Id,
                OuterId = payment.OuterId,
                PaymentGatewayCode = payment.PaymentGatewayCode,
                TaxType = payment.TaxType,

                Amount = (double)payment.Amount.InternalAmount,

                Currency = payment.Currency.Code,
                Price = (double)payment.Price.InternalAmount,
                DiscountAmount = (double)payment.DiscountAmount.InternalAmount,
                TaxPercentRate = (double)payment.TaxPercentRate
            };

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

        public static cartDto.Address ToCartAddressDto(this Address address)
        {
            return address.ToCoreAddressDto().JsonConvert<cartDto.Address>();
        }

        public static Address ToAddress(this cartDto.Address addressDto)
        {
            return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public static PromotionEvaluationContext ToPromotionEvaluationContext(this ShoppingCart cart)
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

        public static ShoppingCart ToShoppingCart(this cartDto.ShoppingCart cartDto, Currency currency, Language language, User user)
        {
            var result = new ShoppingCart(currency, language)
            {
                ChannelId = cartDto.ChannelId,
                Comment = cartDto.Comment,
                CustomerId = cartDto.CustomerId,
                CustomerName = cartDto.CustomerName,
                Id = cartDto.Id,
                Name = cartDto.Name,
                ObjectType = cartDto.ObjectType,
                OrganizationId = cartDto.OrganizationId,
                Status = cartDto.Status,
                StoreId = cartDto.StoreId,
                Type = cartDto.Type,
                Customer = user
            };

            if (cartDto.Coupons != null)
            {
                result.Coupons = cartDto.Coupons.Select(c => new Coupon { Code = c, AppliedSuccessfully = !string.IsNullOrEmpty(c) }).ToList();
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

            result.Total = new Money(cartDto.Total ?? 0, currency);
            result.SubTotal = new Money(cartDto.SubTotal ?? 0, currency);
            result.SubTotalWithTax = new Money(cartDto.SubTotalWithTax ?? 0, currency);
            result.ShippingPrice = new Money(cartDto.ShippingSubTotal ?? 0, currency);
            result.ShippingPriceWithTax = new Money(cartDto.ShippingSubTotalWithTax ?? 0, currency);
            result.ShippingTotal = new Money(cartDto.ShippingTotal ?? 0, currency);
            result.ShippingTotalWithTax = new Money(cartDto.ShippingTotalWithTax ?? 0, currency);
            result.PaymentPrice = new Money(cartDto.PaymentSubTotal ?? 0, currency);
            result.PaymentPriceWithTax = new Money(cartDto.PaymentSubTotalWithTax ?? 0, currency);
            result.PaymentTotal = new Money(cartDto.PaymentTotal ?? 0, currency);
            result.PaymentTotalWithTax = new Money(cartDto.PaymentTotalWithTax ?? 0, currency);

            result.DiscountTotal = new Money(cartDto.DiscountTotal ?? 0, currency);
            result.DiscountTotalWithTax = new Money(cartDto.DiscountTotalWithTax ?? 0, currency);
            result.TaxTotal = new Money(cartDto.TaxTotal ?? 0, currency);


            result.IsAnonymous = cartDto.IsAnonymous == true;
            result.IsRecuring = cartDto.IsRecuring == true;
            result.VolumetricWeight = (decimal)(cartDto.VolumetricWeight ?? 0);
            result.Weight = (decimal)(cartDto.Weight ?? 0);

            return result;
        }

        public static cartDto.ShoppingCart ToShoppingCartDto(this ShoppingCart cart)
        {
            var result = new cartDto.ShoppingCart
            {
                ChannelId = cart.ChannelId,
                Comment = cart.Comment,
                CustomerId = cart.CustomerId,
                CustomerName = cart.CustomerName,
                Id = cart.Id,
                Name = cart.Name,
                ObjectType = cart.ObjectType,
                OrganizationId = cart.OrganizationId,
                Status = cart.Status,
                StoreId = cart.StoreId,
                Type = cart.Type,
                IsAnonymous = cart.IsAnonymous
            };

            if (cart.Language != null)
            {
                result.LanguageCode = cart.Language.CultureName;
            }
            result.Addresses = cart.Addresses.Select(ToCartAddressDto).ToList();
            result.Coupons = cart.Coupons?.Select(c => c.Code).ToList();
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

        public static TaxEvaluationContext ToTaxEvalContext(this ShoppingCart cart, Store store)
        {
            var result = new TaxEvaluationContext(cart.StoreId)
            {
                Id = cart.Id,
                Code = cart.Name,
                Currency = cart.Currency,
                Type = "Cart",
                Customer = cart.Customer,
                StoreTaxCalculationEnabled = store.TaxCalculationEnabled,
                FixedTaxRate = store.FixedTaxRate
            };

            foreach (var lineItem in cart.Items)
            {
                result.Lines.Add(new TaxLine(lineItem.Currency)
                {
                    Id = lineItem.Id,
                    Code = lineItem.Sku,
                    Name = lineItem.Name,
                    TaxType = lineItem.TaxType,
                    //Special case when product have 100% discount and need to calculate tax for old value
                    Amount = lineItem.ExtendedPrice.Amount > 0 ? lineItem.ExtendedPrice : lineItem.SalePrice,
                    Quantity = lineItem.Quantity,
                    Price = lineItem.PlacedPrice,
                    TypeName = "item"
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
                    Amount = shipment.Total.Amount > 0 ? shipment.Total : shipment.Price,
                    TypeName = "shipment"
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
                    Amount = payment.Total.Amount > 0 ? payment.Total : payment.Price,
                    TypeName = "payment"
                };
                result.Lines.Add(totalTaxLine);
            }
            return result;
        }

        public static TaxDetail ToTaxDetail(this cartDto.TaxDetail taxDeatilDto, Currency currency)
        {
            var result = new TaxDetail(currency)
            {
                Name = taxDeatilDto.Name,
                Rate = new Money(taxDeatilDto.Rate ?? 0, currency),
                Amount = new Money(taxDeatilDto.Amount ?? 0, currency),
            };
            return result;
        }

        public static cartDto.TaxDetail ToCartTaxDetailDto(this TaxDetail taxDetail)
        {
            var result = new cartDto.TaxDetail
            {
                Name = taxDetail.Name,
                Rate = (double)taxDetail.Rate.Amount,
                Amount = (double)taxDetail.Amount.Amount,
            };
            return result;
        }

        public static LineItem ToLineItem(this Product product, Language language, int quantity)
        {
            var result = new LineItem(product.Price.Currency, language)
            {
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Sku = product.Sku,
                ProductType = product.ProductType,
                TaxType = product.TaxType,
                WeightUnit = product.WeightUnit,
                MeasureUnit = product.MeasureUnit,
                Weight = product.Weight,
                Width = product.Width,
                Length = product.Length,
                Height = product.Height,

                ImageUrl = product.PrimaryImage?.Url,
                ThumbnailImageUrl = product.PrimaryImage?.Url,
                ListPrice = product.Price.ListPrice,
                SalePrice = product.Price.GetTierPrice(quantity).Price,
                TaxPercentRate = product.Price.TaxPercentRate,
                DiscountAmount = product.Price.DiscountAmount,
                ProductId = product.Id,
                Quantity = quantity
            };
            result.IsReccuring = result.PaymentPlan != null;

            return result;
        }

        public static LineItem ToLineItem(this cartDto.LineItem lineItemDto, Currency currency, Language language)
        {
            var result = new LineItem(currency, language)
            {
                Id = lineItemDto.Id,
                IsReadOnly = lineItemDto.IsReadOnly ?? false,
                CatalogId = lineItemDto.CatalogId,
                CategoryId = lineItemDto.CategoryId,
                ImageUrl = lineItemDto.ImageUrl,
                Name = lineItemDto.Name,
                ObjectType = lineItemDto.ObjectType,
                ProductId = lineItemDto.ProductId,
                ProductType = lineItemDto.ProductType,
                Quantity = lineItemDto.Quantity ?? 1,
                ShipmentMethodCode = lineItemDto.ShipmentMethodCode,
                Sku = lineItemDto.Sku,
                TaxType = lineItemDto.TaxType,
                ThumbnailImageUrl = lineItemDto.ThumbnailImageUrl,
                WeightUnit = lineItemDto.WeightUnit,
                MeasureUnit = lineItemDto.MeasureUnit,
                Weight = (decimal?)lineItemDto.Weight,
                Width = (decimal?)lineItemDto.Width,
                Length = (decimal?)lineItemDto.Length,
                Height = (decimal?)lineItemDto.Height,
            };



            result.ImageUrl = lineItemDto.ImageUrl.RemoveLeadingUriScheme();

            if (lineItemDto.TaxDetails != null)
            {
                result.TaxDetails = lineItemDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            if (lineItemDto.DynamicProperties != null)
            {
                result.DynamicProperties = new MutablePagedList<DynamicProperty>(lineItemDto.DynamicProperties.Select(ToDynamicProperty).ToList());
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


            result.DiscountAmountWithTax = new Money(lineItemDto.DiscountAmountWithTax ?? 0, currency);
            result.DiscountTotal = new Money(lineItemDto.DiscountTotal ?? 0, currency);
            result.DiscountTotalWithTax = new Money(lineItemDto.DiscountTotalWithTax ?? 0, currency);
            result.ListPriceWithTax = new Money(lineItemDto.ListPriceWithTax ?? 0, currency);
            result.SalePriceWithTax = new Money(lineItemDto.SalePriceWithTax ?? 0, currency);
            result.PlacedPrice = new Money(lineItemDto.PlacedPrice ?? 0, currency);
            result.PlacedPriceWithTax = new Money(lineItemDto.PlacedPriceWithTax ?? 0, currency);
            result.ExtendedPrice = new Money(lineItemDto.ExtendedPrice ?? 0, currency);
            result.ExtendedPriceWithTax = new Money(lineItemDto.ExtendedPriceWithTax ?? 0, currency);
            result.TaxTotal = new Money(lineItemDto.TaxTotal ?? 0, currency);

            return result;
        }

        public static cartDto.LineItem ToLineItemDto(this LineItem lineItem)
        {
            var retVal = new cartDto.LineItem
            {
                Id = lineItem.Id,
                IsReadOnly = lineItem.IsReadOnly,
                CatalogId = lineItem.CatalogId,
                CategoryId = lineItem.CategoryId,
                ImageUrl = lineItem.ImageUrl,
                Name = lineItem.Name,
                ObjectType = lineItem.ObjectType,
                ProductId = lineItem.ProductId,
                ProductType = lineItem.ProductType,
                Quantity = lineItem.Quantity,
                ShipmentMethodCode = lineItem.ShipmentMethodCode,
                Sku = lineItem.Sku,
                TaxType = lineItem.TaxType,
                ThumbnailImageUrl = lineItem.ThumbnailImageUrl,
                WeightUnit = lineItem.WeightUnit,
                MeasureUnit = lineItem.MeasureUnit,
                Weight = (double?)lineItem.Weight,
                Width = (double?)lineItem.Width,
                Length = (double?)lineItem.Length,
                Height = (double?)lineItem.Height,
                Note = lineItem.Comment,

                Currency = lineItem.Currency.Code,
                Discounts = lineItem.Discounts.Select(ToCartDiscountDto).ToList(),

                ListPrice = (double)lineItem.ListPrice.InternalAmount,
                SalePrice = (double)lineItem.SalePrice.InternalAmount,
                TaxPercentRate = (double)lineItem.TaxPercentRate,
                DiscountAmount = (double)lineItem.DiscountAmount.InternalAmount,
                TaxDetails = lineItem.TaxDetails.Select(ToCartTaxDetailDto).ToList(),
                DynamicProperties = lineItem.DynamicProperties.Select(ToCartDynamicPropertyDto).ToList(),
                VolumetricWeight = (double)(lineItem.VolumetricWeight ?? 0)
            };
            retVal.Weight = (double?)lineItem.Weight;
            retVal.Width = (double?)lineItem.Width;
            retVal.Height = (double?)lineItem.Height;
            retVal.Length = (double?)lineItem.Length;

            return retVal;
        }

        public static CartShipmentItem ToShipmentItem(this LineItem lineItem)
        {
            var shipmentItem = new CartShipmentItem
            {
                LineItem = lineItem,
                Quantity = lineItem.Quantity
            };
            return shipmentItem;
        }

        public static marketingDto.ProductPromoEntry ToProductPromoEntryDto(this LineItem lineItem)
        {
            var result = new marketingDto.ProductPromoEntry
            {
                CatalogId = lineItem.CatalogId,
                CategoryId = lineItem.CategoryId,
                Code = lineItem.Sku,
                ProductId = lineItem.ProductId,
                Discount = (double)lineItem.DiscountTotal.Amount,
                //Use only base price for discount evaluation
                Price = (double)lineItem.SalePrice.Amount,
                Quantity = lineItem.Quantity,
                InStockQuantity = lineItem.InStockQuantity,
                Outline = lineItem.Product.Outline,
                Variations = null // TODO
            };

            return result;
        }
    }
}
