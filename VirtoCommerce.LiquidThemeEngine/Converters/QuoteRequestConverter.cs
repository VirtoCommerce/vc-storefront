using System.Collections.Generic;
using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class QuoteRequestConverter
    {
        public static QuoteRequest ToShopifyModel(this Storefront.Model.Quote.QuoteRequest quoteRequest)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidQuoteRequest(quoteRequest);
        }

        public static QuoteItem ToShopifyModel(this Storefront.Model.Quote.QuoteItem quoteItem)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidQuoteItem(quoteItem);
        }

        public static QuoteRequestTotals ToShopifyModel(this Storefront.Model.Quote.QuoteRequestTotals totals)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidRequestTotal(totals);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual QuoteRequest ToLiquidQuoteRequest(Storefront.Model.Quote.QuoteRequest quoteRequest)
        {
            var result = new QuoteRequest();

            result.CancelledDate = quoteRequest.CancelledDate;
            result.CancelReason = quoteRequest.CancelReason;
            result.ChannelId = quoteRequest.ChannelId;
            result.Comment = quoteRequest.Comment;
            result.CreatedBy = quoteRequest.CreatedBy;
            result.CreatedDate = quoteRequest.CreatedDate;
            result.CustomerId = quoteRequest.CustomerId;
            result.CustomerName = quoteRequest.CustomerName;
            result.EmployeeId = quoteRequest.EmployeeId;
            result.EmployeeName = quoteRequest.EmployeeName;
            result.EnableNotification = quoteRequest.EnableNotification;
            result.ExpirationDate = quoteRequest.ExpirationDate;
            result.Id = quoteRequest.Id;
            result.IsAnonymous = quoteRequest.IsAnonymous;
            result.IsCancelled = quoteRequest.IsCancelled;
            result.IsLocked = quoteRequest.IsLocked;
            result.ModifiedBy = quoteRequest.ModifiedBy;
            result.ModifiedDate = quoteRequest.ModifiedDate;
            result.Number = quoteRequest.Number;
            result.ObjectType = quoteRequest.ObjectType;
            result.OrganizationId = quoteRequest.OrganizationId;
            result.OrganizationName = quoteRequest.OrganizationName;
            result.ReminderDate = quoteRequest.ReminderDate;
            result.Status = quoteRequest.Status;
            result.StoreId = quoteRequest.StoreId;
            
            result.Addresses = new List<Address>();
            foreach (var address in quoteRequest.Addresses)
            {
                result.Addresses.Add(ToLiquidAddress(address));
            }

            result.Attachments = new List<Attachment>();
            foreach (var attachment in quoteRequest.Attachments)
            {
                result.Attachments.Add(ToLiquidAttachment(attachment));
            }

            if (quoteRequest.Coupon != null)
            {
                result.Coupon = quoteRequest.Coupon.Code;
            }

            result.Currency = ToLiquidCurrency(quoteRequest.Currency);

            result.Items = new List<QuoteItem>();
            foreach (var quoteItem in quoteRequest.Items)
            {
                result.Items.Add(ToLiquidQuoteItem(quoteItem));
            }

            result.Language = ToLiquidLanguage(quoteRequest.Language);
            result.ManualRelDiscountAmount = quoteRequest.ManualRelDiscountAmount.Amount;
            result.ManualShippingTotal = quoteRequest.ManualShippingTotal.Amount;
            result.ManualSubTotal = quoteRequest.ManualSubTotal.Amount;

            if (quoteRequest.ShipmentMethod != null)
            {
                result.ShipmentMethod = ToLiquidShippingMethod(quoteRequest.ShipmentMethod);
            }

            result.TaxDetails = new List<TaxLine>();
            foreach (var taxDetail in quoteRequest.TaxDetails)
            {
                result.TaxDetails.Add(ToLiquidTaxLine(taxDetail));
            }

            if (quoteRequest.Totals != null)
            {
                result.Totals = ToLiquidRequestTotal(quoteRequest.Totals);
            }

            return result;
        }

        public virtual QuoteItem ToLiquidQuoteItem(Storefront.Model.Quote.QuoteItem quoteItem)
        {
            var result = new QuoteItem();

            result.CatalogId = quoteItem.CatalogId;
            result.CategoryId = quoteItem.CategoryId;
            result.Comment = quoteItem.Comment;
            result.CreatedBy = quoteItem.CreatedBy;
            result.CreatedDate = quoteItem.CreatedDate;
            result.Id = quoteItem.Id;
            result.ImageUrl = quoteItem.ImageUrl;
            result.ModifiedBy = quoteItem.ModifiedBy;
            result.ModifiedDate = quoteItem.ModifiedDate;
            result.Name = quoteItem.Name;
            result.ProductId = quoteItem.ProductId;
            result.Sku = quoteItem.Sku;
            result.TaxType = quoteItem.TaxType;            

            result.Currency = ToLiquidCurrency(quoteItem.Currency);
            result.ListPrice = quoteItem.ListPrice.Amount * 100;

            result.ProposalPrices = new List<TierPrice>();
            foreach (var proposalPrice in quoteItem.ProposalPrices)
            {
                result.ProposalPrices.Add(ToLiquidTierPrice(proposalPrice));
            }

            result.SalePrice = quoteItem.SalePrice.Amount * 100;

            if (quoteItem.SelectedTierPrice != null)
            {
                result.SelectedTierPrice = ToLiquidTierPrice(quoteItem.SelectedTierPrice);
            }

            return result;
        }

        public virtual QuoteRequestTotals ToLiquidRequestTotal(Storefront.Model.Quote.QuoteRequestTotals requestTotal)
        {
            var result = new QuoteRequestTotals();

            result.AdjustmentQuoteExlTax = requestTotal.AdjustmentQuoteExlTax.Amount * 100;
            result.DiscountTotal = requestTotal.DiscountTotal.Amount * 100;
            result.GrandTotalExlTax = requestTotal.GrandTotalExlTax.Amount * 100;
            result.GrandTotalInclTax = requestTotal.GrandTotalInclTax.Amount * 100;
            result.OriginalSubTotalExlTax = requestTotal.OriginalSubTotalExlTax.Amount * 100;
            result.ShippingTotal = requestTotal.ShippingTotal.Amount * 100;
            result.SubTotalExlTax = requestTotal.SubTotalExlTax.Amount * 100;
            result.TaxTotal = requestTotal.TaxTotal.Amount * 100;

            return result;
        }
    }
}