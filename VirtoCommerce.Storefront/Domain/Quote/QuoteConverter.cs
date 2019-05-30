using System;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Quote;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using quoteDto = VirtoCommerce.Storefront.AutoRestClients.QuoteModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class QuoteConverterExtension
    {
        public static QuoteConverter QuoteConverterInstance
        {
            get
            {
                return new QuoteConverter();
            }
        }
        public static quoteDto.QuoteRequestSearchCriteria ToQuoteSearchCriteriaDto(this QuoteSearchCriteria criteria)
        {
            return QuoteConverterInstance.ToQuoteSearchCriteriaDto(criteria);
        }

        public static QuoteRequest ToQuoteRequest(this quoteDto.QuoteRequest quoteRequestDto, Currency currency, Language language)
        {
            return QuoteConverterInstance.ToQuoteRequest(quoteRequestDto, currency, language);
        }

        public static quoteDto.QuoteRequest ToQuoteRequestDto(this QuoteRequest quoteRequest)
        {
            return QuoteConverterInstance.ToQuoteRequestDto(quoteRequest);
        }

        public static TaxDetail ToTaxDetail(this quoteDto.TaxDetail taxDetailDto, Currency currency)
        {
            return QuoteConverterInstance.ToTaxDetail(taxDetailDto, currency);
        }

        public static quoteDto.TaxDetail ToQuoteTaxDetailDto(this TaxDetail taxDetail)
        {
            return QuoteConverterInstance.ToQuoteTaxDetailDto(taxDetail);
        }

        public static quoteDto.QuoteAttachment ToQuoteAttachmentDto(this Attachment attachment)
        {
            return QuoteConverterInstance.ToQuoteAttachmentDto(attachment);
        }

        public static TierPrice ToTierPrice(this quoteDto.TierPrice tierPriceDto, Currency currency)
        {
            return QuoteConverterInstance.ToTierPrice(tierPriceDto, currency);
        }

        public static quoteDto.TierPrice ToQuoteTierPriceDto(this TierPrice tierPrice)
        {
            return QuoteConverterInstance.ToQuoteTierPriceDto(tierPrice);
        }

        public static QuoteItem ToQuoteItem(this Product product, long quantity, Currency currency)
        {
            return QuoteConverterInstance.ToQuoteItem(product, quantity, currency);
        }

        public static QuoteItem ToQuoteItem(this quoteDto.QuoteItem quoteItemDto, Currency currency)
        {
            return QuoteConverterInstance.ToQuoteItem(quoteItemDto, currency);
        }

        public static quoteDto.QuoteItem ToQuoteItemDto(this QuoteItem quoteItem)
        {
            return QuoteConverterInstance.ToQuoteItemDto(quoteItem);
        }

        public static QuoteRequestTotals ToQuoteTotals(this quoteDto.QuoteRequestTotals totalsDto, Currency currency)
        {
            return QuoteConverterInstance.ToQuoteTotals(totalsDto, currency);
        }

        public static quoteDto.QuoteRequestTotals ToQuoteTotalsDto(this QuoteRequestTotals totals)
        {
            return QuoteConverterInstance.ToQuoteTotalsDto(totals);
        }

        public static ShippingMethod ToShippingMethod(this quoteDto.ShipmentMethod shippingMethodDto, Currency currency)
        {
            return QuoteConverterInstance.ToShippingMethod(shippingMethodDto, currency);
        }

        public static Address ToAddress(this quoteDto.QuoteAddress addressDto)
        {
            return QuoteConverterInstance.ToAddress(addressDto);
        }

        public static quoteDto.QuoteAddress ToQuoteAddressDto(this Address address)
        {
            return QuoteConverterInstance.ToQuoteAddressDto(address);
        }

        public static Attachment ToAttachment(this quoteDto.QuoteAttachment attachmentDto)
        {
            return QuoteConverterInstance.ToAttachment(attachmentDto);
        }

        public static DynamicProperty ToDynamicProperty(this quoteDto.DynamicObjectProperty propertyDto)
        {
            return QuoteConverterInstance.ToDynamicProperty(propertyDto);
        }

        public static quoteDto.DynamicObjectProperty ToQuoteDynamicPropertyDto(this DynamicProperty property)
        {
            return QuoteConverterInstance.ToQuoteDynamicPropertyDto(property);
        }
    }

    public partial class QuoteConverter
    {
        public virtual quoteDto.QuoteRequestSearchCriteria ToQuoteSearchCriteriaDto(QuoteSearchCriteria criteria)
        {
            var result = new quoteDto.QuoteRequestSearchCriteria
            {
                CustomerId = criteria.CustomerId,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                Sort = criteria.Sort
            };
            return result;
        }

        public virtual DynamicProperty ToDynamicProperty(quoteDto.DynamicObjectProperty propertyDto)
        {
            return propertyDto.JsonConvert<coreDto.DynamicObjectProperty>().ToDynamicProperty();
        }

        public virtual quoteDto.DynamicObjectProperty ToQuoteDynamicPropertyDto(DynamicProperty property)
        {
            return property.ToDynamicPropertyDto().JsonConvert<quoteDto.DynamicObjectProperty>();
        }

        public virtual Attachment ToAttachment(quoteDto.QuoteAttachment attachmentDto)
        {
            var result = new Attachment
            {
                CreatedBy = attachmentDto.CreatedBy,
                CreatedDate = attachmentDto.CreatedDate,
                Id = attachmentDto.Id,
                MimeType = attachmentDto.MimeType,
                ModifiedBy = attachmentDto.ModifiedBy,
                ModifiedDate = attachmentDto.ModifiedDate,
                Name = attachmentDto.Name,
                Size = attachmentDto.Size,
                Url = attachmentDto.Url
            };
            return result;
        }

        public virtual Address ToAddress(quoteDto.QuoteAddress addressDto)
        {

            var retVal = new Address
            {
                City = addressDto.City,
                CountryCode = addressDto.CountryCode,
                CountryName = addressDto.CountryName,
                Email = addressDto.Email,
                FirstName = addressDto.FirstName,
                LastName = addressDto.LastName,
                Line1 = addressDto.Line1,
                Line2 = addressDto.Line2,
                MiddleName = addressDto.MiddleName,
                Organization = addressDto.Organization,
                Phone = addressDto.Phone,
                PostalCode = addressDto.PostalCode,
                RegionId = addressDto.RegionId,
                RegionName = addressDto.RegionName,
                Zip = addressDto.Zip,

                Type = (AddressType)Enum.Parse(typeof(AddressType), addressDto.AddressType, true)
            };
            return retVal;

            //return addressDto.JsonConvert<coreDto.Address>().ToAddress();
        }

        public virtual quoteDto.QuoteAddress ToQuoteAddressDto(Address address)
        {
            var result = new quoteDto.QuoteAddress
            {
                City = address.City,
                CountryCode = address.CountryCode,
                CountryName = address.CountryName,
                Email = address.Email,
                FirstName = address.FirstName,
                LastName = address.LastName,
                Line1 = address.Line1,
                Line2 = address.Line2,
                MiddleName = address.MiddleName,
                Organization = address.Organization,
                Phone = address.Phone,
                PostalCode = address.PostalCode,
                RegionId = address.RegionId,
                RegionName = address.RegionName,
                Zip = address.Zip,

                AddressType = address.Type.ToString()
            };

            return result;
            //return address.ToCoreAddressDto().JsonConvert<quoteDto.Address>();
        }

        public virtual QuoteRequest ToQuoteRequest(quoteDto.QuoteRequest quoteRequestDto, Currency currency, Language language)
        {
            var result = new QuoteRequest(currency, language)
            {
                CancelledDate = quoteRequestDto.CancelledDate,
                CancelReason = quoteRequestDto.CancelReason,
                ChannelId = quoteRequestDto.ChannelId,
                Comment = quoteRequestDto.Comment,
                CreatedBy = quoteRequestDto.CreatedBy,
                CreatedDate = quoteRequestDto.CreatedDate,
                CustomerId = quoteRequestDto.CustomerId,
                CustomerName = quoteRequestDto.CustomerName,
                EmployeeId = quoteRequestDto.EmployeeId,
                EmployeeName = quoteRequestDto.EmployeeName,
                EnableNotification = quoteRequestDto.EnableNotification ?? false,
                ExpirationDate = quoteRequestDto.ExpirationDate,
                Id = quoteRequestDto.Id,
                IsAnonymous = quoteRequestDto.IsAnonymous ?? false,
                IsCancelled = quoteRequestDto.IsCancelled ?? false,
                IsLocked = quoteRequestDto.IsLocked ?? false,
                ModifiedBy = quoteRequestDto.ModifiedBy,
                ModifiedDate = quoteRequestDto.ModifiedDate,
                Number = quoteRequestDto.Number,
                OrganizationId = quoteRequestDto.OrganizationId,
                OrganizationName = quoteRequestDto.OrganizationName,
                ReminderDate = quoteRequestDto.ReminderDate,
                Status = quoteRequestDto.Status,
                StoreId = quoteRequestDto.StoreId,
                Tag = quoteRequestDto.Tag,

                Currency = currency,
                Language = language,
                ManualRelDiscountAmount = new Money(quoteRequestDto.ManualRelDiscountAmount ?? 0, currency),
                ManualShippingTotal = new Money(quoteRequestDto.ManualShippingTotal ?? 0, currency),
                ManualSubTotal = new Money(quoteRequestDto.ManualSubTotal ?? 0, currency)
            };

            if (quoteRequestDto.Addresses != null)
            {
                result.Addresses = quoteRequestDto.Addresses.Select(a => ToAddress(a)).ToList();
            }

            if (quoteRequestDto.Attachments != null)
            {
                result.Attachments = quoteRequestDto.Attachments.Select(a => ToAttachment(a)).ToList();
            }

            if (!string.IsNullOrEmpty(quoteRequestDto.Coupon))
            {
                result.Coupon = new Coupon { AppliedSuccessfully = true, Code = quoteRequestDto.Coupon };
            }

            if (quoteRequestDto.DynamicProperties != null)
            {
                result.DynamicProperties = quoteRequestDto.DynamicProperties.Select(ToDynamicProperty).ToList();
            }

            if (quoteRequestDto.Items != null)
            {
                result.Items = quoteRequestDto.Items.Select(i => ToQuoteItem(i, currency)).ToList();
            }

            // TODO
            if (quoteRequestDto.ShipmentMethod != null)
            {
            }

            if (quoteRequestDto.TaxDetails != null)
            {
                result.TaxDetails = quoteRequestDto.TaxDetails.Select(td => ToTaxDetail(td, currency)).ToList();
            }

            if (quoteRequestDto.Totals != null)
            {
                result.Totals = ToQuoteTotals(quoteRequestDto.Totals, currency);
            }

            return result;
        }

        public virtual quoteDto.QuoteRequest ToQuoteRequestDto(QuoteRequest quoteRequest)
        {
            var result = new quoteDto.QuoteRequest
            {
                CancelledDate = quoteRequest.CancelledDate,
                CancelReason = quoteRequest.CancelReason,
                ChannelId = quoteRequest.ChannelId,
                Comment = quoteRequest.Comment,
                CreatedBy = quoteRequest.CreatedBy,
                CreatedDate = quoteRequest.CreatedDate,
                CustomerId = quoteRequest.CustomerId,
                CustomerName = quoteRequest.CustomerName,
                EmployeeId = quoteRequest.EmployeeId,
                EmployeeName = quoteRequest.EmployeeName,
                EnableNotification = quoteRequest.EnableNotification,
                ExpirationDate = quoteRequest.ExpirationDate,
                Id = quoteRequest.Id,
                IsAnonymous = quoteRequest.IsAnonymous,
                IsCancelled = quoteRequest.IsCancelled,
                IsLocked = quoteRequest.IsLocked,
                ModifiedBy = quoteRequest.ModifiedBy,
                ModifiedDate = quoteRequest.ModifiedDate,
                Number = quoteRequest.Number,
                OrganizationId = quoteRequest.OrganizationId,
                OrganizationName = quoteRequest.OrganizationName,
                ReminderDate = quoteRequest.ReminderDate,
                Status = quoteRequest.Status,
                StoreId = quoteRequest.StoreId,
                Tag = quoteRequest.Tag,

                Currency = quoteRequest.Currency.Code,
                Addresses = quoteRequest.Addresses.Select(ToQuoteAddressDto).ToList(),
                Attachments = quoteRequest.Attachments.Select(ToQuoteAttachmentDto).ToList(),
                DynamicProperties = quoteRequest.DynamicProperties.Select(ToQuoteDynamicPropertyDto).ToList(),
                Items = quoteRequest.Items.Select(ToQuoteItemDto).ToList(),
                LanguageCode = quoteRequest.Language.CultureName,
                ManualRelDiscountAmount = quoteRequest.ManualRelDiscountAmount != null ? (double?)quoteRequest.ManualRelDiscountAmount.Amount : null,
                ManualShippingTotal = quoteRequest.ManualShippingTotal != null ? (double?)quoteRequest.ManualShippingTotal.Amount : null,
                ManualSubTotal = quoteRequest.ManualSubTotal != null ? (double?)quoteRequest.ManualSubTotal.Amount : null,
                TaxDetails = quoteRequest.TaxDetails.Select(ToQuoteTaxDetailDto).ToList()
            };

            if (quoteRequest.Coupon != null && quoteRequest.Coupon.AppliedSuccessfully)
            {
                result.Coupon = quoteRequest.Coupon.Code;
            }

            if (quoteRequest.Totals != null)
            {
                result.Totals = ToQuoteTotalsDto(quoteRequest.Totals);
            }

            return result;
        }

        public virtual QuoteItem ToQuoteItem(quoteDto.QuoteItem quoteItemDto, Currency currency)
        {
            var result = new QuoteItem
            {
                CatalogId = quoteItemDto.CatalogId,
                CategoryId = quoteItemDto.CategoryId,
                Comment = quoteItemDto.Comment,
                CreatedBy = quoteItemDto.CreatedBy,
                CreatedDate = quoteItemDto.CreatedDate,
                Id = quoteItemDto.Id,
                ImageUrl = quoteItemDto.ImageUrl,
                ModifiedBy = quoteItemDto.ModifiedBy,
                ModifiedDate = quoteItemDto.ModifiedDate,
                Name = quoteItemDto.Name,
                ProductId = quoteItemDto.ProductId,
                Sku = quoteItemDto.Sku,
                TaxType = quoteItemDto.TaxType,

                Currency = currency,
                ListPrice = new Money(quoteItemDto.ListPrice ?? 0, currency),
                SalePrice = new Money(quoteItemDto.SalePrice ?? 0, currency)
            };

            if (quoteItemDto.ProposalPrices != null)
            {
                result.ProposalPrices = quoteItemDto.ProposalPrices.Select(pp => ToTierPrice(pp, currency)).ToList();
            }

            if (quoteItemDto.SelectedTierPrice != null)
            {
                result.SelectedTierPrice = ToTierPrice(quoteItemDto.SelectedTierPrice, currency);
            }

            return result;
        }

        public virtual quoteDto.QuoteItem ToQuoteItemDto(QuoteItem quoteItem)
        {
            var result = new quoteDto.QuoteItem
            {
                CatalogId = quoteItem.CatalogId,
                CategoryId = quoteItem.CategoryId,
                Comment = quoteItem.Comment,
                CreatedBy = quoteItem.CreatedBy,
                CreatedDate = quoteItem.CreatedDate,
                Id = quoteItem.Id,
                ImageUrl = quoteItem.ImageUrl,
                ModifiedBy = quoteItem.ModifiedBy,
                ModifiedDate = quoteItem.ModifiedDate,
                Name = quoteItem.Name,
                ProductId = quoteItem.ProductId,
                Sku = quoteItem.Sku,
                TaxType = quoteItem.TaxType,

                Currency = quoteItem.Currency.Code,
                ListPrice = (double)quoteItem.ListPrice.Amount,
                ProposalPrices = quoteItem.ProposalPrices.Select(ToQuoteTierPriceDto).ToList(),
                SalePrice = (double)quoteItem.SalePrice.Amount
            };

            if (quoteItem.SelectedTierPrice != null)
            {
                result.SelectedTierPrice = ToQuoteTierPriceDto(quoteItem.SelectedTierPrice);
            }

            return result;
        }

        public virtual QuoteItem ToQuoteItem(Product product, long quantity, Currency currency)
        {
            var retVal = new QuoteItem
            {
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Sku = product.Sku,
                TaxType = product.TaxType,
                Currency = currency,

                ImageUrl = product.PrimaryImage != null ? product.PrimaryImage.Url : null,
                ListPrice = product.Price.ListPrice,
                ProductId = product.Id,
                SalePrice = product.Price.SalePrice
            };
            retVal.ProposalPrices.Add(new TierPrice(product.Price.SalePrice, quantity));
            retVal.SelectedTierPrice = retVal.ProposalPrices.First();

            return retVal;
        }

        public virtual QuoteRequestTotals ToQuoteTotals(quoteDto.QuoteRequestTotals totalsDto, Currency currency)
        {
            var result = new QuoteRequestTotals(currency)
            {
                AdjustmentQuoteExlTax = new Money(totalsDto.AdjustmentQuoteExlTax ?? 0, currency),
                DiscountTotal = new Money(totalsDto.DiscountTotal ?? 0, currency),
                GrandTotalExlTax = new Money(totalsDto.GrandTotalExlTax ?? 0, currency),
                GrandTotalInclTax = new Money(totalsDto.GrandTotalInclTax ?? 0, currency),
                OriginalSubTotalExlTax = new Money(totalsDto.OriginalSubTotalExlTax ?? 0, currency),
                ShippingTotal = new Money(totalsDto.ShippingTotal ?? 0, currency),
                SubTotalExlTax = new Money(totalsDto.SubTotalExlTax ?? 0, currency),
                TaxTotal = new Money(totalsDto.TaxTotal ?? 0, currency)
            };
            return result;
        }

        public virtual quoteDto.QuoteRequestTotals ToQuoteTotalsDto(QuoteRequestTotals totals)
        {
            var result = new quoteDto.QuoteRequestTotals
            {
                AdjustmentQuoteExlTax = (double)totals.AdjustmentQuoteExlTax.Amount,
                DiscountTotal = (double)totals.DiscountTotal.Amount,
                GrandTotalExlTax = (double)totals.GrandTotalExlTax.Amount,
                GrandTotalInclTax = (double)totals.GrandTotalInclTax.Amount,
                OriginalSubTotalExlTax = (double)totals.OriginalSubTotalExlTax.Amount,
                ShippingTotal = (double)totals.ShippingTotal.Amount,
                SubTotalExlTax = (double)totals.SubTotalExlTax.Amount,
                TaxTotal = (double)totals.TaxTotal.Amount
            };

            return result;
        }

        public virtual ShippingMethod ToShippingMethod(quoteDto.ShipmentMethod shippingMethodDto, Currency currency)
        {
            var result = new ShippingMethod(currency)
            {
                LogoUrl = shippingMethodDto.LogoUrl,
                Name = shippingMethodDto.Name,
                OptionName = shippingMethodDto.OptionName
            };
            result.LogoUrl = shippingMethodDto.LogoUrl;
            result.Price = new Money(shippingMethodDto.Price ?? 0, currency);
            return result;
        }

        public virtual TaxDetail ToTaxDetail(quoteDto.TaxDetail taxDetail, Currency currency)
        {
            var result = new TaxDetail(currency)
            {
                Amount = new Money(taxDetail.Amount ?? 0, currency),
                Rate = new Money(taxDetail.Rate ?? 0, currency),
                Name = taxDetail.Name
            };
            return result;
        }

        public virtual quoteDto.TaxDetail ToQuoteTaxDetailDto(TaxDetail taxDetail)
        {
            var result = new quoteDto.TaxDetail
            {
                Amount = (double)taxDetail.Amount.Amount,
                Name = taxDetail.Name,
                Rate = (double)taxDetail.Rate.Amount
            };
            return result;
        }

        public virtual quoteDto.QuoteAttachment ToQuoteAttachmentDto(Attachment attachment)
        {
            var result = new quoteDto.QuoteAttachment
            {
                CreatedBy = attachment.CreatedBy,
                CreatedDate = attachment.CreatedDate,
                Id = attachment.Id,
                MimeType = attachment.MimeType,
                ModifiedBy = attachment.ModifiedBy,
                ModifiedDate = attachment.ModifiedDate,
                Name = attachment.Name,
                Size = attachment.Size,
                Url = attachment.Url
            };
            return result;
        }

        public virtual TierPrice ToTierPrice(quoteDto.TierPrice tierPriceDto, Currency currency)
        {
            var result = new TierPrice(currency)
            {
                Quantity = tierPriceDto.Quantity ?? 1,
                Price = new Money(tierPriceDto.Price ?? 0, currency)
            };
            return result;
        }

        public virtual quoteDto.TierPrice ToQuoteTierPriceDto(TierPrice webModel)
        {
            var result = new quoteDto.TierPrice
            {
                Quantity = webModel.Quantity,
                Price = (double)webModel.Price.Amount
            };
            return result;
        }

    }
}
