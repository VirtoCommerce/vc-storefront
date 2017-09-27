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
    public static class SubscriptionConverterExtension
    {
        public static SubscriptionConverter SubscriptionConverterInstance
        {
            get
            {
                return new SubscriptionConverter();
            }
        }

        public static Subscription ToSubscription(this subscriptionDto.Subscription subscriptionDto, ICollection<Currency> availCurrencies, Language language)
        {
            return SubscriptionConverterInstance.ToSubscription(subscriptionDto, availCurrencies, language);
        }
        public static PaymentPlan ToPaymentPlan(this subscriptionDto.PaymentPlan paymentPlanDto)
        {
            return SubscriptionConverterInstance.ToPaymentPlan(paymentPlanDto);
        }
        public static subscriptionDto.PaymentPlan ToPaymentPlanDto(this PaymentPlan paymentPlan)
        {
            return SubscriptionConverterInstance.ToPaymentPlanDto(paymentPlan);
        }
        public static subscriptionDto.SubscriptionSearchCriteria ToSearchCriteriaDto(this SubscriptionSearchCriteria criteria)
        {
            return SubscriptionConverterInstance.ToSearchCriteriaDto(criteria);
        }

    }

    public class SubscriptionConverter
    {
        public virtual subscriptionDto.PaymentPlan ToPaymentPlanDto(PaymentPlan paymentPlan)
        {
            var result = new subscriptionDto.PaymentPlan();
            result.Id = paymentPlan.Id;
            result.Interval = paymentPlan.Interval.ToString();
            result.IntervalCount = paymentPlan.IntervalCount;
            result.TrialPeriodDays = paymentPlan.TrialPeriodDays;
            return result;
        }

        public virtual PaymentPlan ToPaymentPlan(subscriptionDto.PaymentPlan paymentPlanDto)
        {
            var result = new PaymentPlan();
            result.Id = paymentPlanDto.Id;
            result.Interval = EnumUtility.SafeParse(paymentPlanDto.Interval, PaymentInterval.Months);
            result.IntervalCount = paymentPlanDto.IntervalCount ?? 0;
            result.TrialPeriodDays = paymentPlanDto.TrialPeriodDays ?? 0;
            return result;
        }

        public virtual subscriptionDto.SubscriptionSearchCriteria ToSearchCriteriaDto(SubscriptionSearchCriteria criteria)
        {
            var result = new subscriptionDto.SubscriptionSearchCriteria();

            result.CustomerId = criteria.CustomerId;          
            result.Sort = criteria.Sort;
            result.Number = criteria.Number;
            result.Skip = criteria.Start;
            result.Take = criteria.PageSize;
            result.Sort = criteria.Sort;

            return result;
        }

        public virtual Subscription ToSubscription(subscriptionDto.Subscription subscriptionDto, ICollection<Currency> availCurrencies, Language language)
        {
            var currency = availCurrencies.FirstOrDefault(x => x.Equals(subscriptionDto.CustomerOrderPrototype.Currency)) ?? new Currency(language, subscriptionDto.CustomerOrderPrototype.Currency);
            var order = subscriptionDto.CustomerOrderPrototype.JsonConvert<orderDto.CustomerOrder>()
                                                                .ToCustomerOrder(availCurrencies, language);
            var result = new Subscription(currency);
            result.Addresses = order.Addresses;
            result.ChannelId = order.ChannelId;
            result.Comment = order.Comment;
            result.CreatedBy = order.CreatedBy;
            result.CreatedDate = order.CreatedDate;
            result.CustomerId = order.CustomerId;
            result.CustomerName = order.CustomerName;
            result.Discount = order.Discount;
            result.DiscountAmount = order.DiscountAmount;
            result.DiscountAmountWithTax = order.DiscountAmountWithTax;
            result.Discounts = order.Discounts;
            result.DiscountTotal = order.DiscountTotal;
            result.DiscountTotalWithTax = order.DiscountTotalWithTax;
            result.DynamicProperties = order.DynamicProperties;
            result.EmployeeId = order.EmployeeId;
            result.EmployeeName = order.EmployeeName;
            result.InPayments = order.InPayments;
            result.Items = order.Items;
            result.ModifiedBy = order.ModifiedBy;
            result.ModifiedDate = order.ModifiedDate;
            result.OrganizationId = order.OrganizationId;
            result.OrganizationName = order.OrganizationName;
            result.Shipments = order.Shipments;
            result.ShippingDiscountTotal = order.ShippingDiscountTotal;
            result.ShippingDiscountTotalWithTax = order.ShippingDiscountTotalWithTax;
            result.ShippingTaxTotal = order.ShippingTaxTotal;
            result.ShippingTotal = order.ShippingTotal;
            result.ShippingTotalWithTax = order.ShippingTotalWithTax;
            result.StoreId = order.StoreId;
            result.StoreName = order.StoreName;
            result.SubTotal = order.SubTotal;
            result.SubTotalDiscount = order.SubTotalDiscount;
            result.SubTotalDiscountWithTax = order.SubTotalDiscountWithTax;
            result.SubTotalTaxTotal = order.SubTotalTaxTotal;
            result.SubTotalWithTax = order.SubTotalWithTax;
            result.TaxDetails = order.TaxDetails;
            result.TaxTotal = order.TaxTotal;
            result.Total = order.Total;

            result.Id = subscriptionDto.Id;
            result.Number = subscriptionDto.Number;
            result.Balance = new Money(subscriptionDto.Balance ?? 0, currency);
            result.Interval = EnumUtility.SafeParse(subscriptionDto.Interval, PaymentInterval.Months);
            result.IntervalCount = subscriptionDto.IntervalCount ?? 1;
            result.StartDate = subscriptionDto.StartDate;
            result.EndDate = subscriptionDto.EndDate;
            result.Status = subscriptionDto.SubscriptionStatus;
            result.IsCancelled = subscriptionDto.IsCancelled;
            result.CancelReason = subscriptionDto.CancelReason;
            result.CancelledDate = subscriptionDto.CancelledDate;
            result.TrialSart = subscriptionDto.TrialSart;
            result.TrialEnd = subscriptionDto.TrialEnd;
            result.TrialPeriodDays = subscriptionDto.TrialPeriodDays ?? 0;
            result.CurrentPeriodStart = subscriptionDto.CurrentPeriodStart;
            result.CurrentPeriodEnd = subscriptionDto.CurrentPeriodEnd;

            if (subscriptionDto.CustomerOrders != null)
            {
                foreach (var relatedOrderDto in subscriptionDto.CustomerOrders)
                {
                    var relatedOrder = new CustomerOrder(currency);
                    relatedOrder.Id = relatedOrderDto.Id;
                    relatedOrder.Number = relatedOrderDto.Number;
                    relatedOrder.Total = new Money(relatedOrderDto.Total ?? 0, currency);
                    relatedOrder.CreatedDate = relatedOrderDto.CreatedDate;
                    relatedOrder.Status = relatedOrderDto.Status;
                    result.CustomerOrders.Add(relatedOrder);
                }
            }

            return result;
        }

    }
}
