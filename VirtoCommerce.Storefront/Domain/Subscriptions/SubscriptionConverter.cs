using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Subscriptions;
using orderDto = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;
using subscriptionDto = VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{

    public static partial class SubscriptionConverter
    {
        public static subscriptionDto.PaymentPlan ToPaymentPlanDto(this PaymentPlan paymentPlan)
        {
            var result = new subscriptionDto.PaymentPlan
            {
                Id = paymentPlan.Id,
                Interval = paymentPlan.Interval.ToString(),
                IntervalCount = paymentPlan.IntervalCount,
                TrialPeriodDays = paymentPlan.TrialPeriodDays
            };
            return result;
        }

        public static PaymentPlan ToPaymentPlan(this subscriptionDto.PaymentPlan paymentPlanDto)
        {
            var result = new PaymentPlan
            {
                Id = paymentPlanDto.Id,
                Interval = EnumUtility.SafeParse(paymentPlanDto.Interval, PaymentInterval.Months),
                IntervalCount = paymentPlanDto.IntervalCount ?? 0,
                TrialPeriodDays = paymentPlanDto.TrialPeriodDays ?? 0
            };
            return result;
        }

        public static subscriptionDto.SubscriptionSearchCriteria ToSearchCriteriaDto(this SubscriptionSearchCriteria criteria)
        {
            var result = new subscriptionDto.SubscriptionSearchCriteria
            {
                CustomerId = criteria.CustomerId,
                Sort = criteria.Sort,
                Number = criteria.Number,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                ResponseGroup = ((int)criteria.ResponseGroup).ToString(),
                ModifiedSinceDate = criteria.ModifiedSinceDate
            };
            result.Sort = criteria.Sort;

            return result;
        }

        public static Subscription ToSubscription(this subscriptionDto.Subscription subscriptionDto, IEnumerable<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(subscriptionDto.CustomerOrderPrototype.Currency)) ?? new Currency(language, subscriptionDto.CustomerOrderPrototype.Currency);
            var order = subscriptionDto.CustomerOrderPrototype.JsonConvert<orderDto.CustomerOrder>()
                                                                .ToCustomerOrder(availCurrencies, language);
            var result = new Subscription(currency)
            {
                Addresses = order.Addresses,
                ChannelId = order.ChannelId,
                Comment = order.Comment,
                CreatedBy = order.CreatedBy,
                CreatedDate = order.CreatedDate,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                Discount = order.Discount,
                DiscountAmount = order.DiscountAmount,
                DiscountAmountWithTax = order.DiscountAmountWithTax,
                Discounts = order.Discounts,
                DiscountTotal = order.DiscountTotal,
                DiscountTotalWithTax = order.DiscountTotalWithTax,
                DynamicProperties = order.DynamicProperties,
                EmployeeId = order.EmployeeId,
                EmployeeName = order.EmployeeName,
                InPayments = order.InPayments,
                Items = order.Items,
                ModifiedBy = order.ModifiedBy,
                ModifiedDate = order.ModifiedDate,
                OrganizationId = order.OrganizationId,
                OrganizationName = order.OrganizationName,
                Shipments = order.Shipments,
                ShippingDiscountTotal = order.ShippingDiscountTotal,
                ShippingDiscountTotalWithTax = order.ShippingDiscountTotalWithTax,
                ShippingTaxTotal = order.ShippingTaxTotal,
                ShippingTotal = order.ShippingTotal,
                ShippingTotalWithTax = order.ShippingTotalWithTax,
                StoreId = order.StoreId,
                StoreName = order.StoreName,
                SubTotal = order.SubTotal,
                SubTotalDiscount = order.SubTotalDiscount,
                SubTotalDiscountWithTax = order.SubTotalDiscountWithTax,
                SubTotalTaxTotal = order.SubTotalTaxTotal,
                SubTotalWithTax = order.SubTotalWithTax,
                TaxDetails = order.TaxDetails,
                TaxTotal = order.TaxTotal,
                Total = order.Total,

                Id = subscriptionDto.Id,
                Number = subscriptionDto.Number,
                Balance = new Money(subscriptionDto.Balance ?? 0, currency),
                Interval = EnumUtility.SafeParse(subscriptionDto.Interval, PaymentInterval.Months),
                IntervalCount = subscriptionDto.IntervalCount ?? 1,
                StartDate = subscriptionDto.StartDate,
                EndDate = subscriptionDto.EndDate,
                Status = subscriptionDto.SubscriptionStatus,
                IsCancelled = subscriptionDto.IsCancelled,
                CancelReason = subscriptionDto.CancelReason,
                CancelledDate = subscriptionDto.CancelledDate,
                TrialSart = subscriptionDto.TrialSart,
                TrialEnd = subscriptionDto.TrialEnd,
                TrialPeriodDays = subscriptionDto.TrialPeriodDays ?? 0,
                CurrentPeriodStart = subscriptionDto.CurrentPeriodStart,
                CurrentPeriodEnd = subscriptionDto.CurrentPeriodEnd
            };

            if (subscriptionDto.CustomerOrders != null)
            {
                foreach (var relatedOrderDto in subscriptionDto.CustomerOrders)
                {
                    var relatedOrder = new CustomerOrder(currency)
                    {
                        Id = relatedOrderDto.Id,
                        Number = relatedOrderDto.Number,
                        Total = new Money(relatedOrderDto.Total ?? 0, currency),
                        CreatedDate = relatedOrderDto.CreatedDate,
                        Status = relatedOrderDto.Status
                    };
                    result.CustomerOrders.Add(relatedOrder);
                }
            }

            return result;
        }

    }
}
