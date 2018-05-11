using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;
using Discount = VirtoCommerce.LiquidThemeEngine.Objects.Discount;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class OrderConverter
    {
        public static Order ToShopifyModel(this CustomerOrder order, Storefront.Model.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidOrder(order, language, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Order ToLiquidOrder(CustomerOrder order, Storefront.Model.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new Order();
            result.Cancelled = order.IsCancelled == true;
            result.CancelledAt = order.CancelledDate;
            result.CancelReason = order.CancelReason;
            result.CancelReasonLabel = order.CancelReason;
            result.CreatedAt = order.CreatedDate ?? DateTime.MinValue;
            result.Name = order.Number;
            result.OrderNumber = order.Number;
            result.CurrencyCode = order.Currency.Code;
            result.CustomerUrl = urlBuilder.ToAppAbsolute("/account/order/" + order.Number);

            if (order.Addresses != null)
            {
                var shippingAddress = order.Addresses
                    .FirstOrDefault(a => (a.Type & AddressType.Shipping) == AddressType.Shipping);

                if (shippingAddress != null)
                {
                    result.ShippingAddress = ToLiquidAddress(shippingAddress);
                }

                var billingAddress = order.Addresses
                    .FirstOrDefault(a => (a.Type & AddressType.Billing) == AddressType.Billing);

                if (billingAddress != null)
                {
                    result.BillingAddress = ToLiquidAddress(billingAddress);
                }
                else if (shippingAddress != null)
                {
                    result.BillingAddress = ToLiquidAddress(shippingAddress);
                }

                result.Email = order.Addresses
                    .Where(a => !string.IsNullOrEmpty(a.Email))
                    .Select(a => a.Email)
                    .FirstOrDefault();
            }


            var discountTotal = order.DiscountTotal;
            var discounts = new List<Discount>();

            if (order.Discount != null)
            {
                discounts.Add(ToLiquidDiscount(order.Discount));
            }

            var taxLines = new List<Objects.TaxLine>();

            if (order.InPayments != null)
            {
                var inPayment = order.InPayments.OrderByDescending(p => p.CreatedDate)
                    .FirstOrDefault();

                if (inPayment != null)
                {
                    if (string.IsNullOrEmpty(inPayment.Status))
                    {
                        result.FinancialStatus = inPayment.IsApproved == true ? "Paid" : "Pending";
                        result.FinancialStatusLabel = inPayment.IsApproved == true ? "Paid" : "Pending";
                    }
                    else
                    {
                        result.FinancialStatus = inPayment.Status;
                        result.FinancialStatusLabel = inPayment.Status;
                    }
                }
            }

            if (order.Shipments != null)
            {
                result.ShippingMethods = order.Shipments.Select(s => ToLiquidShippingMethod(s)).ToArray();
                result.ShippingPrice = result.ShippingMethods.Sum(s => s.Price);
                result.ShippingPriceWithTax = result.ShippingMethods.Sum(s => s.PriceWithTax);

                var orderShipment = order.Shipments.FirstOrDefault();

                if (orderShipment != null)
                {
                    if (string.IsNullOrEmpty(orderShipment.Status))
                    {
                        result.FulfillmentStatus = orderShipment.IsApproved == true ? "Sent" : "Not sent";
                        result.FulfillmentStatusLabel = orderShipment.IsApproved == true ? "Sent" : "Not sent";
                    }
                    else
                    {
                        result.FulfillmentStatus = orderShipment.Status;
                        result.FulfillmentStatusLabel = orderShipment.Status;
                    }
                }

                if (order.ShippingTaxTotal.Amount > 0)
                {
                    taxLines.Add(new Objects.TaxLine
                    {
                        Title = "Shipping",
                        Price = order.ShippingTaxTotal.Amount * 100,
                        Rate = order.Shipments.Average(s => s.TaxPercentRate)
                    });
                }
                if (order.ShippingDiscountTotal.Amount > 0)
                {
                    discounts.Add(new Discount
                    {
                        Type = "ShippingDiscount",
                        Code = "Shipping",
                        Amount = order.ShippingDiscountTotal.Amount * 100,
                    });
                }
            }

            if (order.Items != null)
            {
                result.LineItems = order.Items.Select(i => ToLiquidLineItem(i, language, urlBuilder)).ToArray();
                result.SubtotalPrice = order.SubTotal.Amount * 100;
                result.SubtotalPriceWithTax = order.SubTotalWithTax.Amount * 100;

                if (order.SubTotalTaxTotal.Amount > 0)
                {
                    taxLines.Add(new Objects.TaxLine
                    {
                        Title = "Line items",
                        Price = order.SubTotalTaxTotal.Amount * 100,
                        Rate = order.Items.Average(i => i.TaxPercentRate)
                    });
                }
            }


            if (order.DiscountAmount.Amount > 0)
            {
                discounts.Add(new Discount
                {
                    Type = "Order subtotal",
                    Code = "Order",
                    Amount = order.DiscountAmount.Amount * 100
                });
            }

            if (!order.InPayments.IsNullOrEmpty())
            {
                result.Transactions = order.InPayments.Select(x => ToLiquidTransaction(x)).ToArray();
            }

            result.TaxLines = taxLines.ToArray();
            result.TaxPrice = taxLines.Sum(t => t.Price);

            result.Discounts = discounts.ToArray();

            result.TotalPrice = order.Total.Amount * 100;

            return result;
        }
    }
}
