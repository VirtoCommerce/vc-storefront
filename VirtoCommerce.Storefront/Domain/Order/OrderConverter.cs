using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Order;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using platformDto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using orderDto = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;
using storeDto = VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi.Models;
using paymentDto = VirtoCommerce.Storefront.AutoRestClients.PaymentModuleApi.Models;
using VirtoCommerce.Storefront.Model.Contracts;
using System;

namespace VirtoCommerce.Storefront.Domain
{

    public static partial class OrderConverter
    {
        public static orderDto.CustomerOrderSearchCriteria ToSearchCriteriaDto(this OrderSearchCriteria criteria)
        {
            var result = new orderDto.CustomerOrderSearchCriteria
            {
                Keyword = criteria.Keyword,
                CustomerId = criteria.CustomerId,
                StartDate = criteria.StartDate,
                EndDate = criteria.EndDate,
                Status = criteria.Status,
                Statuses = criteria.Statuses,
                StoreIds = criteria.StoreIds,

                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };

            return result;
        }

        public static orderDto.PaymentSearchCriteria ToPaymentSearchCriteriaDto(this PaymentSearchCriteria criteria)
        {
            var result = new orderDto.PaymentSearchCriteria
            {
                OrderId = criteria.OrderId,
                OrderNumber = criteria.OrderNumber,
                Keyword = criteria.Keyword,
                Status = criteria.Status,
                Statuses = criteria.Statuses,
                StoreIds = criteria.StoreIds,
                StartDate = criteria.StartDate,
                EndDate = criteria.EndDate,
                CapturedStartDate = criteria.CapturedStartDate,
                CapturedEndDate = criteria.CapturedEndDate,
                AuthorizedStartDate = criteria.AuthorizedStartDate,
                AuthorizedEndDate = criteria.AuthorizedEndDate,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };

            return result;
        }


        public static DynamicProperty ToDynamicProperty(this orderDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<platformDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public static orderDto.DynamicObjectProperty ToOrderDynamicPropertyDto(this DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<orderDto.DynamicObjectProperty>();
        }

        public static orderDto.OrderAddress ToOrderAddressDto(this Address address)
        {
            return address.ToCoreAddressDto().JsonConvert<orderDto.OrderAddress>();
        }

        public static Address ToAddress(this orderDto.OrderAddress addressDto)
        {
            return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public static ShipmentPackage ToShipmentPackage(this orderDto.ShipmentPackage shipmentPackageDto, IEnumerable<Currency> currencies, Language language)
        {
            var result = new ShipmentPackage
            {
                BarCode = shipmentPackageDto.BarCode,
                Id = shipmentPackageDto.Id,

                Height = shipmentPackageDto.Height,
                Weight = shipmentPackageDto.Weight,
                Length = shipmentPackageDto.Length,
                Width = shipmentPackageDto.Width,
                WeightUnit = shipmentPackageDto.WeightUnit,
                MeasureUnit = shipmentPackageDto.MeasureUnit
            };

            if (shipmentPackageDto.Items != null)
            {
                result.Items = shipmentPackageDto.Items.Select(i => ToShipmentItem(i, currencies, language)).ToList();
            }

            return result;
        }

        public static ShipmentPackageDto ToShipmentPackageDto(this ShipmentPackage shipmentPackage)
        {
            var result = new ShipmentPackageDto
            {
                BarCode = shipmentPackage.BarCode,
                Id = shipmentPackage.Id,

                Height = shipmentPackage.Height,
                Weight = shipmentPackage.Weight,
                Length = shipmentPackage.Length,
                Width = shipmentPackage.Width,
                WeightUnit = shipmentPackage.WeightUnit,
                MeasureUnit = shipmentPackage.MeasureUnit
            };

            if (shipmentPackage.Items != null)
            {
                result.Items = shipmentPackage.Items.Select(i => ToShipmentItemDto(i)).ToList();
            }

            return result;
        }

        public static ShipmentItem ToShipmentItem(this orderDto.OrderShipmentItem shipmentItemDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var result = new ShipmentItem();

            result.BarCode = shipmentItemDto.BarCode;
            result.Id = shipmentItemDto.Id;
            result.LineItemId = shipmentItemDto.LineItemId;
            result.Quantity = shipmentItemDto.Quantity;

            if (shipmentItemDto.LineItem != null)
            {
                result.LineItem = ToOrderLineItem(shipmentItemDto.LineItem, availCurrencies, language);
            }

            return result;
        }

        public static OrderShipmentItemDto ToShipmentItemDto(this ShipmentItem shipmentItem)
        {
            var result = new OrderShipmentItemDto();

            result.BarCode = shipmentItem.BarCode;
            result.Id = shipmentItem.Id;
            result.LineItemId = shipmentItem.LineItemId;
            result.Quantity = shipmentItem.Quantity;

            if (shipmentItem.LineItem != null)
            {
                result.LineItem = ToOrderLineItemDto(shipmentItem.LineItem);
            }

            return result;
        }

        public static Shipment ToOrderShipment(this orderDto.OrderShipment shipmentDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(shipmentDto.Currency)) ?? new Currency(language, shipmentDto.Currency);
            var result = new Shipment(currency)
            {
                CancelledDate = shipmentDto.CancelledDate,
                CancelReason = shipmentDto.CancelReason,
                Comment = shipmentDto.Comment,
                EmployeeId = shipmentDto.EmployeeId,
                EmployeeName = shipmentDto.EmployeeName,
                FulfillmentCenterId = shipmentDto.FulfillmentCenterId,
                FulfillmentCenterName = shipmentDto.FulfillmentCenterName,
                Height = shipmentDto.Height,
                Id = shipmentDto.Id,
                Length = shipmentDto.Length,
                MeasureUnit = shipmentDto.MeasureUnit,
                Number = shipmentDto.Number,
                OperationType = shipmentDto.OperationType,
                OrganizationId = shipmentDto.OrganizationId,
                OrganizationName = shipmentDto.OrganizationName,
                ShipmentMethodCode = shipmentDto.ShipmentMethodCode,
                ShipmentMethodOption = shipmentDto.ShipmentMethodOption,
                Status = shipmentDto.Status,
                TaxType = shipmentDto.TaxType,
                Weight = shipmentDto.Weight,
                WeightUnit = shipmentDto.WeightUnit,
                Width = shipmentDto.Width,
                Currency = currency,
                CreatedBy = shipmentDto.CreatedBy,
                CreatedDate = shipmentDto.CreatedDate,
                ModifiedDate = shipmentDto.ModifiedDate,
                ModifiedBy = shipmentDto.ModifiedBy,
            };

            if (shipmentDto.DeliveryAddress != null)
            {
                result.DeliveryAddress = ToAddress(shipmentDto.DeliveryAddress);
            }

            if (!shipmentDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(shipmentDto.Discounts.Select(x => ToDiscount(x, new[] { currency }, language)));
            }

            if (shipmentDto.DynamicProperties != null)
            {
                result.DynamicProperties = shipmentDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            if (shipmentDto.InPayments != null)
            {
                result.InPayments = shipmentDto.InPayments.Select(p => ToOrderInPayment(p, availCurrencies, language)).ToList();
            }

            if (shipmentDto.Items != null)
            {
                result.Items = shipmentDto.Items.Select(i => ToShipmentItem(i, availCurrencies, language)).ToList();
            }

            if (shipmentDto.Packages != null)
            {
                result.Packages = shipmentDto.Packages.Select(p => ToShipmentPackage(p, availCurrencies, language)).ToList();
            }

            result.Price = new Money(shipmentDto.Price ?? 0, currency);
            result.PriceWithTax = new Money(shipmentDto.PriceWithTax ?? 0, currency);
            result.DiscountAmount = new Money(shipmentDto.DiscountAmount ?? 0, currency);
            result.DiscountAmountWithTax = new Money(shipmentDto.DiscountAmountWithTax ?? 0, currency);
            result.Total = new Money(shipmentDto.Total ?? 0, currency);
            result.TotalWithTax = new Money(shipmentDto.TotalWithTax ?? 0, currency);
            result.TaxTotal = new Money(shipmentDto.TaxTotal ?? 0, currency);
            result.TaxPercentRate = (decimal?)shipmentDto.TaxPercentRate ?? 0m;
            if (shipmentDto.TaxDetails != null)
            {
                result.TaxDetails = shipmentDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }
            return result;
        }

        public static OrderShipmentDto ToOrderShipmentDto(this Shipment shipment)
        {
            var result = new OrderShipmentDto()
            {
                CancelledDate = shipment.CancelledDate,
                CancelReason = shipment.CancelReason,
                Comment = shipment.Comment,
                EmployeeId = shipment.EmployeeId,
                EmployeeName = shipment.EmployeeName,
                FulfillmentCenterId = shipment.FulfillmentCenterId,
                FulfillmentCenterName = shipment.FulfillmentCenterName,
                Height = shipment.Height,
                Id = shipment.Id,
                IsCancelled = shipment.IsCancelled.GetValueOrDefault(),
                IsApproved = shipment.IsApproved.GetValueOrDefault(),
                Length = shipment.Length,
                MeasureUnit = shipment.MeasureUnit,
                Number = shipment.Number,
                OperationType = shipment.OperationType,
                OrganizationId = shipment.OrganizationId,
                OrganizationName = shipment.OrganizationName,
                ShipmentMethodCode = shipment.ShipmentMethodCode,
                ShipmentMethodOption = shipment.ShipmentMethodOption,
                Status = shipment.Status,
                TaxType = shipment.TaxType,
                Weight = shipment.Weight,
                WeightUnit = shipment.WeightUnit,
                Width = shipment.Width,
                Currency = shipment.Currency.Code,
                //CreatedBy = shipment.CreatedBy,
                //CreatedDate = shipment.CreatedDate,
                //ModifiedDate = shipment.ModifiedDate,
                //ModifiedBy = shipment.ModifiedBy,
                Price = (double?)shipment.Price.Amount,
                PriceWithTax = (double?)shipment.PriceWithTax.Amount,
                DiscountAmount = (double?)shipment.DiscountAmount.Amount,
                DiscountAmountWithTax = (double?)shipment.DiscountAmountWithTax.Amount,
                Total = (double?)shipment.Total.Amount,
                TotalWithTax = (double?)shipment.TotalWithTax.Amount,
                TaxTotal = (double?)shipment.TaxTotal.Amount,
                TaxPercentRate = (double?)shipment.TaxPercentRate,
            };

            if (shipment.DeliveryAddress != null)
            {
                result.DeliveryAddress = shipment.DeliveryAddress.ToDto();
            }

            if (!shipment.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(shipment.Discounts.Select(x => x.ToDiscountDto()).ToList());
            }

            //TODO
            //if (shipment.DynamicProperties != null)
            //{
            //    result.DynamicProperties = shipment.DynamicProperties.Select(ToOrderDynamicPropertyDto).ToList();
            //}

            if (shipment.InPayments != null)
            {
                result.InPayments = shipment.InPayments.Select(p => ToOrderPaymentInDto(p)).ToList();
            }

            if (shipment.Items != null)
            {
                result.Items = shipment.Items.Select(i => ToShipmentItemDto(i)).ToList();
            }

            if (shipment.Packages != null)
            {
                result.Packages = shipment.Packages.Select(p => ToShipmentPackageDto(p)).ToList();
            }

            
            if (shipment.TaxDetails != null)
            {
                result.TaxDetails = shipment.TaxDetails.Select(p => ToTaxDetailDto(p)).ToList();
            }
            return result;
        }

        public static LineItem ToOrderLineItem(this orderDto.OrderLineItem lineItemDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(lineItemDto.Currency)) ?? new Currency(language, lineItemDto.Currency);

            var result = new LineItem(currency)
            {
                CancelledDate = lineItemDto.CancelledDate,
                CancelReason = lineItemDto.CancelReason,
                CatalogId = lineItemDto.CatalogId,
                CategoryId = lineItemDto.CategoryId,
                Height = lineItemDto.Height,
                Id = lineItemDto.Id,
                ImageUrl = lineItemDto.ImageUrl,
                IsCancelled = lineItemDto.IsCancelled,
                IsGift = lineItemDto.IsGift,
                Length = lineItemDto.Length,
                MeasureUnit = lineItemDto.MeasureUnit,
                Name = lineItemDto.Name,
                ProductId = lineItemDto.ProductId,
                Quantity = lineItemDto.Quantity,
                ReserveQuantity = lineItemDto.ReserveQuantity,
                ShippingMethodCode = lineItemDto.ShippingMethodCode,
                Sku = lineItemDto.Sku,
                TaxType = lineItemDto.TaxType,
                Weight = lineItemDto.Weight,
                WeightUnit = lineItemDto.WeightUnit,
                Width = lineItemDto.Width,
                CreatedBy = lineItemDto.CreatedBy,
                CreatedDate = lineItemDto.CreatedDate,
                ModifiedDate = lineItemDto.ModifiedDate,
                ModifiedBy = lineItemDto.ModifiedBy
            };


            result.ImageUrl = result.ImageUrl.RemoveLeadingUriScheme();
            result.Currency = currency;
            result.DiscountAmount = new Money(lineItemDto.DiscountAmount ?? 0, currency);

            if (lineItemDto.DynamicProperties != null)
            {
                result.DynamicProperties = lineItemDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }
            result.ListPrice = new Money(lineItemDto.Price ?? 0, currency);
            result.ListPriceWithTax = new Money(lineItemDto.PriceWithTax ?? 0, currency);
            result.DiscountAmount = new Money(lineItemDto.DiscountAmount ?? 0, currency);
            result.DiscountAmountWithTax = new Money(lineItemDto.DiscountAmountWithTax ?? 0, currency);
            result.PlacedPrice = new Money(lineItemDto.PlacedPrice ?? 0, currency);
            result.PlacedPriceWithTax = new Money(lineItemDto.PlacedPriceWithTax ?? 0, currency);
            result.ExtendedPrice = new Money(lineItemDto.ExtendedPrice ?? 0, currency);
            result.ExtendedPriceWithTax = new Money(lineItemDto.ExtendedPriceWithTax ?? 0, currency);
            result.DiscountTotal = new Money(lineItemDto.DiscountTotal ?? 0, currency);
            result.DiscountTotalWithTax = new Money(lineItemDto.DiscountTotalWithTax ?? 0, currency);
            result.TaxTotal = new Money(lineItemDto.TaxTotal ?? 0, currency);
            result.TaxPercentRate = (decimal?)lineItemDto.TaxPercentRate ?? 0m;
            if (!lineItemDto.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(lineItemDto.Discounts.Select(x => ToDiscount(x, new[] { currency }, language)));
            }
            if (lineItemDto.TaxDetails != null)
            {
                result.TaxDetails = lineItemDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            return result;
        }

        public static OrderLineItemDto ToOrderLineItemDto(this LineItem lineItem)
        {
            var result = new OrderLineItemDto()
            {
                CancelledDate = lineItem.CancelledDate,
                CancelReason = lineItem.CancelReason,
                CatalogId = lineItem.CatalogId,
                CategoryId = lineItem.CategoryId,
                Height = lineItem.Height,
                Id = lineItem.Id,
                ImageUrl = lineItem.ImageUrl,
                IsCancelled = lineItem.IsCancelled,
                IsGift = lineItem.IsGift,
                Length = lineItem.Length,
                MeasureUnit = lineItem.MeasureUnit,
                Name = lineItem.Name,
                ProductId = lineItem.ProductId,
                Quantity = lineItem.Quantity,
                ReserveQuantity = lineItem.ReserveQuantity,
                ShippingMethodCode = lineItem.ShippingMethodCode,
                Sku = lineItem.Sku,
                TaxType = lineItem.TaxType,
                Weight = lineItem.Weight,
                WeightUnit = lineItem.WeightUnit,
                Width = lineItem.Width,
                //CreatedBy = lineItem.CreatedBy,
                //CreatedDate = lineItem.CreatedDate,
                //ModifiedDate = lineItem.ModifiedDate,
                //ModifiedBy = lineItem.ModifiedBy,
                Currency = lineItem.Currency.Code,
                Price = (double?)lineItem.ListPrice.Amount,
                PriceWithTax = (double?)lineItem.ListPriceWithTax.Amount,
                DiscountAmount = (double?)lineItem.DiscountAmount.Amount,
                DiscountAmountWithTax = (double?)lineItem.DiscountAmountWithTax.Amount,
                PlacedPrice = (double?)lineItem.PlacedPrice.Amount,
                PlacedPriceWithTax = (double?)lineItem.PlacedPriceWithTax.Amount,
                ExtendedPrice = (double?)lineItem.ExtendedPrice.Amount,
                ExtendedPriceWithTax = (double?)lineItem.ExtendedPriceWithTax.Amount,
                DiscountTotal = (double?)lineItem.DiscountTotal.Amount,
                DiscountTotalWithTax = (double?)lineItem.DiscountTotalWithTax.Amount,
                TaxTotal = (double?)lineItem.TaxTotal.Amount,
                TaxPercentRate = (double?)lineItem.TaxPercentRate,
            };

            result.ImageUrl = result.ImageUrl.RemoveLeadingUriScheme();

            //if (!lineItem.DynamicProperties.IsNullOrEmpty())
            //{
            //    result.DynamicProperties = lineItem.DynamicProperties.Select(ToOrderDynamicPropertyDto).ToList();
            //}
            
            if (!lineItem.Discounts.IsNullOrEmpty())
            {
                result.Discounts = lineItem.Discounts.Select(x => x.ToDiscountDto()).ToList();
            }
            if (!lineItem.TaxDetails.IsNullOrEmpty())
            {
                result.TaxDetails = lineItem.TaxDetails.Select(p => ToTaxDetailDto(p)).ToList();
            }

            return result;
        }

        public static PaymentIn ToOrderInPayment(this orderDto.PaymentIn paymentIn, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(paymentIn.Currency)) ?? new Currency(language, paymentIn.Currency);
            var retVal = new PaymentIn(currency)
            {
                CancelledDate = paymentIn.CancelledDate,
                CancelReason = paymentIn.CancelReason,
                Comment = paymentIn.Comment,
                CustomerId = paymentIn.CustomerId,
                CustomerName = paymentIn.CustomerName,
                GatewayCode = paymentIn.GatewayCode,
                Id = paymentIn.Id,
                IncomingDate = paymentIn.IncomingDate,
                IsApproved = paymentIn.IsApproved,
                IsCancelled = paymentIn.IsCancelled,
                CreatedBy = paymentIn.CreatedBy,
                CreatedDate = paymentIn.CreatedDate,
                ModifiedDate = paymentIn.ModifiedDate,
                ModifiedBy = paymentIn.ModifiedBy,
                Number = paymentIn.Number
            };
            retVal.IsCancelled = paymentIn.IsCancelled;
            retVal.Number = paymentIn.Number;
            retVal.OperationType = paymentIn.OperationType;
            retVal.OrganizationId = paymentIn.OrganizationId;
            retVal.OrganizationName = paymentIn.OrganizationName;
            retVal.OuterId = paymentIn.OuterId;
            retVal.ParentOperationId = paymentIn.ParentOperationId;
            retVal.Purpose = paymentIn.Purpose;
            retVal.Status = paymentIn.Status;
            retVal.CapturedDate = paymentIn.CapturedDate;
            retVal.AuthorizedDate = paymentIn.AuthorizedDate;
            retVal.VoidedDate = paymentIn.VoidedDate;
            retVal.OrderId = paymentIn.OrderId;


            if (paymentIn.BillingAddress != null)
            {
                retVal.BillingAddress = paymentIn.BillingAddress.ToAddress();
            }

            if (paymentIn.DynamicProperties != null)
            {
                retVal.DynamicProperties = paymentIn.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            retVal.Sum = new Money(paymentIn.Sum ?? 0, currency);

            if (paymentIn.PaymentMethod != null)
            {
                retVal.GatewayCode = paymentIn.PaymentMethod.Code;
                retVal.PaymentMethodType = paymentIn.PaymentMethod.PaymentMethodType;
            }
            return retVal;
        }

        public static OrderPaymentInDto ToOrderPaymentInDto(this PaymentIn payment)
        {
            var retVal = new OrderPaymentInDto
            {
                CancelledDate = payment.CancelledDate,
                CancelReason = payment.CancelReason,
                Comment = payment.Comment,
                CustomerId = payment.CustomerId,
                CustomerName = payment.CustomerName,
                GatewayCode = payment.GatewayCode,
                Id = payment.Id,
                IncomingDate = payment.IncomingDate,
                IsApproved = payment.IsApproved,
                IsCancelled = payment.IsCancelled,
                //CreatedBy = payment.CreatedBy,
                //CreatedDate = payment.CreatedDate,
                //ModifiedDate = payment.ModifiedDate,
                //ModifiedBy = payment.ModifiedBy,
                Number = payment.Number,
                TaxTotal = (double?)payment.Tax.Amount,
                Sum = (double)payment.Sum.Amount,
                OperationType = payment.OperationType,
                OrganizationId = payment.OrganizationId,
                OrganizationName = payment.OrganizationName,
                OuterId = payment.OuterId,
                ParentOperationId = payment.ParentOperationId,
                Purpose = payment.Purpose,
                Status = payment.Status,
                Currency = payment.Currency.Code,
                PaymentStatus = (int)Enum.Parse(typeof(PaymentStatus), payment.Status, true)
            };

            if (payment.BillingAddress != null)
            {
                retVal.BillingAddress = payment.BillingAddress.ToDto();
            }

            //if (payment.DynamicProperties != null)
            //{
            //    retVal.DynamicProperties = payment.DynamicProperties.Select(ToOrderDynamicPropertyDto).ToList();
            //}

            //if (payment.GatewayCode != null)
            //{
            //    var a = retVal.GatewayCode;
            //}

            return retVal;
        }
        public static orderDto.BankCardInfo ToBankCardInfoDto(this BankCardInfo model)
        {
            orderDto.BankCardInfo retVal = null;
            if (model != null)
            {
                retVal = new orderDto.BankCardInfo
                {
                    BankCardCVV2 = model.BankCardCVV2,
                    BankCardMonth = model.BankCardMonth,
                    BankCardNumber = model.BankCardNumber,
                    BankCardType = model.BankCardType,
                    BankCardYear = model.BankCardYear,
                    CardholderName = model.CardholderName
                };
            }

            return retVal;
        }

        public static Discount ToDiscount(this orderDto.Discount discountDto, IEnumerable<Currency> availCurrencies, Language language)
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

        public static DiscountDto ToDiscountDto(this Discount discount)
        {
            var result = new DiscountDto()
            {
                Coupon = discount.Coupon,
                Description = discount.Description,
                PromotionId = discount.PromotionId,
                Currency = discount.Amount.Currency.Code,
                DiscountAmount = (double?)discount.Amount.Amount,
                DiscountAmountWithTax = (double?)discount.Amount.Amount
            };

            return result;
        }

        public static CustomerOrderRequestDto ToCustomerOrderDto(this CustomerOrder order)
        {
            var result = new CustomerOrderRequestDto()
            {
                CancelledDate = order.CancelledDate,
                CancelReason = order.CancelReason,
                ChannelId = order.ChannelId,
                Comment = order.Comment,
                //CreatedBy = order.CreatedBy,
                //CreatedDate = order.CreatedDate,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                EmployeeId = order.EmployeeId,
                EmployeeName = order.EmployeeName,
                Id = order.Id,
                IsApproved = order.IsApproved,
                IsCancelled = order.IsCancelled,
                //ModifiedBy = order.ModifiedBy,
                //ModifiedDate = order.ModifiedDate,
                Number = order.Number,
                OrganizationId = order.OrganizationId,
                OrganizationName = order.OrganizationName,
                Status = order.Status,
                StoreId = order.StoreId,
                StoreName = order.StoreName,
                SubscriptionNumber = order.SubscriptionNumber,
                Currency = order.Currency.Code
            };

            if (!order.Addresses.IsNullOrEmpty())
            {
                result.Addresses = order.Addresses.Select(x => x.ToDto()).ToList();
            }

            //TODO
            //if (!order.DynamicProperties.IsNullOrEmpty())
            //{
            //    result.DynamicProperties = order.DynamicProperties;
            //}

            if (!order.InPayments.IsNullOrEmpty())
            {
                result.InPayments = order.InPayments.Select(x => x.ToOrderPaymentInDto()).ToList();
            }

            if (!order.Items.IsNullOrEmpty())
            {
                result.Items = order.Items.Select(x => x.ToOrderLineItemDto()).ToList();
            }

            if (!order.Shipments.IsNullOrEmpty())
            {
                result.Shipments = order.Shipments.Select(x =>
                {
                    var result = x.ToOrderShipmentDto();
                    result.CustomerOrderId = order.Id;
                    return result;
                }).ToList();
            }

            if (!order.Discounts.IsNullOrEmpty())
            {
                result.Discounts = order.Discounts.Select(x => x.ToDiscountDto()).ToList();
            }

            if (!order.TaxDetails.IsNullOrEmpty())
            {
                result.TaxDetails = order.TaxDetails.Select(p => ToTaxDetailDto(p)).ToList();
            }

            result.DiscountAmount = (double?)order.DiscountAmount.Amount;
            result.DiscountTotal = (double?)order.DiscountTotal.Amount;
            result.DiscountTotalWithTax = (double?)order.DiscountTotalWithTax.Amount;

            result.PaymentTotal = (double?)order.PaymentTotal.Amount;
            result.PaymentTotalWithTax = (double?)order.PaymentTotalWithTax.Amount;
            result.PaymentDiscountTotal = (double?)order.PaymentDiscountTotal.Amount;
            result.PaymentDiscountTotalWithTax = (double?)order.PaymentDiscountTotalWithTax.Amount;
            result.PaymentTaxTotal = (double?)order.PaymentTaxTotal.Amount;
            result.PaymentSubTotal = (double?)order.PaymentPrice?.Amount;
            result.PaymentSubTotalWithTax = (double?)order.PaymentPriceWithTax?.Amount;

            result.Total = (double?)order.Total.Amount;
            result.SubTotal = (double?)order.SubTotal.Amount;
            result.SubTotalWithTax = (double?)order.SubTotalWithTax.Amount;
            result.TaxTotal = (double?)order.TaxTotal.Amount;

            result.ShippingTotal = (double?)order.ShippingTotal.Amount;
            result.ShippingTotalWithTax = (double?)order.ShippingTotalWithTax.Amount;
            result.ShippingTaxTotal = (double?)order.ShippingTaxTotal.Amount;
            result.ShippingSubTotal = (double?)order.ShippingPrice?.Amount;
            result.ShippingSubTotalWithTax = (double?)order.ShippingPriceWithTax?.Amount;

            result.SubTotalTaxTotal = (double?)order.SubTotalTaxTotal.Amount;
            result.SubTotalDiscount = (double?)order.SubTotalDiscount.Amount;
            result.SubTotalDiscountWithTax = (double?)order.SubTotalDiscountWithTax.Amount;

            return result;
        }

        public static CustomerOrder ToCustomerOrder(this orderDto.CustomerOrder order, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(order.Currency)) ?? new Currency(language, order.Currency);

            var result = new CustomerOrder(currency)
            {
                CancelledDate = order.CancelledDate,
                CancelReason = order.CancelReason,
                ChannelId = order.ChannelId,
                Comment = order.Comment,
                CreatedBy = order.CreatedBy,
                CreatedDate = order.CreatedDate,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                EmployeeId = order.EmployeeId,
                EmployeeName = order.EmployeeName,
                Id = order.Id,
                IsApproved = order.IsApproved,
                IsCancelled = order.IsCancelled,
                ModifiedBy = order.ModifiedBy,
                ModifiedDate = order.ModifiedDate,
                Number = order.Number,
                OrganizationId = order.OrganizationId,
                OrganizationName = order.OrganizationName,
                Status = order.Status,
                StoreId = order.StoreId,
                StoreName = order.StoreName,
                SubscriptionNumber = order.SubscriptionNumber
            };

            if (order.Addresses != null)
            {
                result.Addresses = order.Addresses.Select(ToAddress).ToList();
            }

            if (order.DynamicProperties != null)
            {
                result.DynamicProperties = order.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            if (order.InPayments != null)
            {
                result.InPayments = order.InPayments.Select(p => ToOrderInPayment(p, availCurrencies, language)).ToList();
            }

            if (order.Items != null)
            {
                result.Items = order.Items.Select(i => ToOrderLineItem(i, availCurrencies, language)).ToList();
            }

            if (order.Shipments != null)
            {
                result.Shipments = order.Shipments.Select(s => ToOrderShipment(s, availCurrencies, language)).ToList();
            }

            if (!order.Discounts.IsNullOrEmpty())
            {
                result.Discounts.AddRange(order.Discounts.Select(x => ToDiscount(x, new[] { currency }, language)));
            }

            if (order.TaxDetails != null)
            {
                result.TaxDetails = order.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            result.DiscountAmount = new Money(order.DiscountAmount ?? 0, currency);
            result.DiscountTotal = new Money(order.DiscountTotal ?? 0, currency);
            result.DiscountTotalWithTax = new Money(order.DiscountTotalWithTax ?? 0, currency);

            result.PaymentTotal = new Money(order.PaymentTotal ?? 0, currency);
            result.PaymentTotalWithTax = new Money(order.PaymentTotalWithTax ?? 0, currency);
            result.PaymentDiscountTotal = new Money(order.PaymentDiscountTotal ?? 0, currency);
            result.PaymentDiscountTotalWithTax = new Money(order.PaymentDiscountTotalWithTax ?? 0, currency);
            result.PaymentTaxTotal = new Money(order.PaymentTaxTotal ?? 0, currency);
            result.PaymentPrice = new Money(order.PaymentSubTotal ?? 0, currency);
            result.PaymentPriceWithTax = new Money(order.PaymentSubTotalWithTax ?? 0, currency);

            result.Total = new Money(order.Total ?? 0, currency);
            result.SubTotal = new Money(order.SubTotal ?? 0, currency);
            result.SubTotalWithTax = new Money(order.SubTotalWithTax ?? 0, currency);
            result.TaxTotal = new Money(order.TaxTotal ?? 0, currency);

            result.ShippingTotal = new Money(order.ShippingTotal ?? 0, currency);
            result.ShippingTotalWithTax = new Money(order.ShippingTotalWithTax ?? 0, currency);
            result.ShippingTaxTotal = new Money(order.ShippingTaxTotal ?? 0, currency);
            result.ShippingPrice = new Money(order.ShippingSubTotal ?? 0, currency);
            result.ShippingPriceWithTax = new Money(order.ShippingSubTotalWithTax ?? 0, currency);

            result.SubTotalTaxTotal = new Money(order.SubTotalTaxTotal ?? 0, currency);
            result.SubTotalDiscount = new Money(order.SubTotalDiscount ?? 0, currency);
            result.SubTotalDiscountWithTax = new Money(order.SubTotalDiscountWithTax ?? 0, currency);


            return result;
        }

        public static TaxDetail ToTaxDetail(this orderDto.TaxDetail taxDetailDto, Currency currency)
        {
            var result = new TaxDetail(currency)
            {
                Name = taxDetailDto.Name,
                Amount = new Money(taxDetailDto.Amount ?? 0, currency),
                Rate = new Money(taxDetailDto.Rate ?? 0, currency),
            };
            return result;
        }

        public static TaxDetailDto ToTaxDetailDto(this TaxDetail taxDetailDto)
        {
            var result = new TaxDetailDto()
            {
                Name = taxDetailDto.Name,
                Amount = (double?)taxDetailDto.Amount.Amount,
                Rate = (double?)taxDetailDto.Rate.Amount,
            };
            return result;
        }

        public static PaymentMethod ToPaymentMethod(this orderDto.PaymentMethod paymentMethodDto, CustomerOrder order)
        {
            return paymentMethodDto.JsonConvert<paymentDto.PaymentMethod>().ToStorePaymentMethod(order.Currency);
        }

        public static PaymentMethod ToPaymentMethod(this PaymentMethod paymentMethodDto, CustomerOrder order)
        {
            return paymentMethodDto.JsonConvert<paymentDto.PaymentMethod>().ToStorePaymentMethod(order.Currency);
        }

        public static ProcessPaymentResult ToProcessPaymentResult(this orderDto.ProcessPaymentRequestResult processPaymentResultDto, CustomerOrder order)
        {
            return new ProcessPaymentResult()
            {
                Error = processPaymentResultDto.ErrorMessage,
                HtmlForm = processPaymentResultDto.HtmlForm,
                IsSuccess = processPaymentResultDto.IsSuccess ?? false,
                NewPaymentStatus = processPaymentResultDto.NewPaymentStatus,
                OuterId = processPaymentResultDto.OuterId,
                PaymentMethod = processPaymentResultDto.PaymentMethod?.ToPaymentMethod(order),
                RedirectUrl = processPaymentResultDto.RedirectUrl
            };
        }
    }
}
