using System;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using catalogDto = VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using marketingDto = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
   
    public static partial class CatalogConverter
    {
        private static MarkdownPipeline _markdownPipeline;
        static CatalogConverter()
        {
            _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }
         
        public static SeoInfo ToSeoInfo(this catalogDto.SeoInfo seoDto)
        {
            return seoDto.JsonConvert<coreDto.SeoInfo>().ToSeoInfo();
        }

        public static Aggregation ToAggregation(this catalogDto.Aggregation aggregationDto, string currentLanguage)
        {
            var result = new Aggregation
            {
                AggregationType = aggregationDto.AggregationType,
                Field = aggregationDto.Field
            };

            if (aggregationDto.Items != null)
            {
                result.Items = aggregationDto.Items
                    .Select(i => i.ToAggregationItem(currentLanguage))
                    .ToArray();
            }

            if (aggregationDto.Labels != null)
            {
                result.Label =
                    aggregationDto.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
                        .Select(l => l.Label)
                        .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(result.Label))
            {
                result.Label = aggregationDto.Field;
            }

            return result;
        }

        public static AggregationItem ToAggregationItem(this catalogDto.AggregationItem itemDto, string currentLanguage)
        {
            var result = new AggregationItem
            {
                Value = itemDto.Value,
                IsApplied = itemDto.IsApplied ?? false,
                Count = itemDto.Count ?? 0,
                Lower = itemDto.RequestedLowerBound,
                Upper = itemDto.RequestedUpperBound,
            };

            if (itemDto.Labels != null)
            {
                result.Label =
                    itemDto.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
                        .Select(l => l.Label)
                        .FirstOrDefault();
            }

            if (string.IsNullOrEmpty(result.Label) && itemDto.Value != null)
            {
                result.Label = itemDto.Value.ToString();
            }

            return result;
        }

        public static CatalogProperty ToProperty(this catalogDto.Property propertyDto, Language currentLanguage)
        {
            var result = new CatalogProperty
            {
                Id = propertyDto.Id,
                Name = propertyDto.Name,
                Type = propertyDto.Type,
                ValueType = propertyDto.ValueType,
                IsMultivalue = propertyDto.Multivalue ?? false
            };

            //Set display names and set current display name for requested language
            if (propertyDto.DisplayNames != null)
            {
                result.DisplayNames = propertyDto.DisplayNames.Select(x => new LocalizedString(new Language(x.LanguageCode), x.Name)).ToList();
                result.DisplayName = result.DisplayNames.FindWithLanguage(currentLanguage, x => x.Value, null);
            }

            //if display name for requested language not set get system property name
            if (string.IsNullOrEmpty(result.DisplayName))
            {
                result.DisplayName = propertyDto.Name;
            }

            //For multilingual properties need populate LocalizedValues collection and set value for requested language
            if (propertyDto.Multilanguage ?? false)
            {
                if (propertyDto.Values != null)
                {
                    result.LocalizedValues = propertyDto.Values.Where(x => x.Value != null).Select(x => new LocalizedString(new Language(x.LanguageCode), x.Value.ToString())).ToList();
                }
            }

            //Set property value
            var propValue = propertyDto.Values?.FirstOrDefault(v => v.Value != null);
            if (propValue != null)
            {
                //Use only one prop value (reserve multi-values to other scenarios)
                result.Value = propValue.Value?.ToString();
                result.ValueId = propValue.ValueId;
            }

            //Try to set value for requested language
            if (result.LocalizedValues.Any())
            {
                result.Value = result.LocalizedValues.FindWithLanguage(currentLanguage, x => x.Value, result.Value);
            }

            //Set multivalues
            if (result.IsMultivalue && propertyDto.Values != null)
            {
                if (result.LocalizedValues.Any())
                {
                    result.Values = result.LocalizedValues.GetLocalizedStringsForLanguage(currentLanguage).Select(x => x.Value).ToList();
                }
                else
                {
                    result.Values = propertyDto.Values.Where(x => x != null).Select(x => x.Value.ToString()).ToList();
                }
            }

            return result;
        }

        public static catalogDto.ProductSearchCriteria ToProductSearchCriteriaDto(this ProductSearchCriteria criteria, WorkContext workContext)
        {
            var result = new catalogDto.ProductSearchCriteria
            {
                SearchPhrase = criteria.Keyword,
                LanguageCode = criteria.Language?.CultureName ?? workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id,
                CatalogId = workContext.CurrentStore.Catalog,
                Outline = criteria.Outline,
                Currency = criteria.Currency?.Code ?? workContext.CurrentCurrency.Code,
                Pricelists = workContext.CurrentPricelists.Where(p => p.Currency.Equals(workContext.CurrentCurrency)).Select(p => p.Id).ToList(),
                PriceRange = criteria.PriceRange?.ToNumericRangeDto(),
                Terms = criteria.Terms.ToStrings(),
                Sort = criteria.SortBy,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                ResponseGroup = ((int)criteria.ResponseGroup).ToString(),
            };

            // Add vendor id to terms
            if (!string.IsNullOrEmpty(criteria.VendorId))
            {
                if (result.Terms == null)
                {
                    result.Terms = new List<string>();
                }

                result.Terms.Add(string.Concat("vendor:", criteria.VendorId));
            }
            // Add user groups to terms
            var contact = workContext.CurrentUser?.Contact;
            if (contact != null && !contact.UserGroups.IsNullOrEmpty())
            {
                if (result.UserGroups == null)
                {
                    result.UserGroups = new List<string>();
                }
                //search products with user_groups defined in customer
                result.UserGroups.AddRange(contact.UserGroups);
            }

            return result;
        }

        public static catalogDto.NumericRange ToNumericRangeDto(this NumericRange range)
        {
            return new catalogDto.NumericRange
            {
                Lower = (double?)range.Lower,
                Upper = (double?)range.Upper,
                IncludeLower = range.IncludeLower,
                IncludeUpper = range.IncludeUpper,
            };
        }

        public static catalogDto.CategorySearchCriteria ToCategorySearchCriteriaDto(this CategorySearchCriteria criteria, WorkContext workContext)
        {
            var result = new catalogDto.CategorySearchCriteria
            {
                SearchPhrase = criteria.Keyword,
                LanguageCode = criteria.Language?.CultureName ?? workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id,
                CatalogId = workContext.CurrentStore.Catalog,
                Outline = criteria.Outline,
                Sort = criteria.SortBy,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                ResponseGroup = ((int)criteria.ResponseGroup).ToString(),
            };
            var contact = workContext.CurrentUser?.Contact;
            if (contact != null && !contact.UserGroups.IsNullOrEmpty())
            {
                if (result.UserGroups == null)
                {
                    result.UserGroups = new List<string>();
                }
                //search categories with user_groups defined in customer
                result.UserGroups.AddRange(contact.UserGroups);
            }

            return result;
        }

        public static Association ToAssociation(this catalogDto.ProductAssociation associationDto)
        {
            Association result = null;

            if (associationDto.AssociatedObjectType.EqualsInvariant("product"))
            {
                result = new ProductAssociation
                {
                    ProductId = associationDto.AssociatedObjectId
                };

            }
            else if (associationDto.AssociatedObjectType.EqualsInvariant("category"))
            {
                result = new CategoryAssociation
                {
                    CategoryId = associationDto.AssociatedObjectId
                };
            }

            if (result != null)
            {
                result.Type = associationDto.Type;
                result.Priority = associationDto.Priority ?? 0;
                result.Image = new Image { Url = associationDto.AssociatedObjectImg };
                result.Quantity = associationDto.Quantity;
            }

            return result;
        }

        public static Category ToCategory(this catalogDto.Category categoryDto, Language currentLanguage, Store store)
        {
            var result = new Category
            {
                Id = categoryDto.Id,
                CatalogId = categoryDto.CatalogId,
                Code = categoryDto.Code,
                Name = categoryDto.Name,
                ParentId = categoryDto.ParentId,
                TaxType = categoryDto.TaxType,
                Outline = categoryDto.Outlines.GetOutlinePath(store.Catalog),
                SeoPath = categoryDto.Outlines.GetSeoPath(store, currentLanguage, null)
            };

            result.Url = "~/" + (result.SeoPath ?? "category/" + categoryDto.Id);

            if (!categoryDto.SeoInfos.IsNullOrEmpty())
            {
                var seoInfoDto = categoryDto.SeoInfos.Select(x => x.JsonConvert<coreDto.SeoInfo>())
                    .GetBestMatchingSeoInfos(store, currentLanguage)
                    .FirstOrDefault();

                if (seoInfoDto != null)
                {
                    result.SeoInfo = seoInfoDto.ToSeoInfo();
                }
            }

            if (result.SeoInfo == null)
            {
                result.SeoInfo = new SeoInfo
                {
                    Slug = categoryDto.Id,
                    Title = categoryDto.Name
                };
            }

            if (categoryDto.Images != null)
            {
                result.Images = categoryDto.Images.Select(ToImage).ToArray();
                result.PrimaryImage = result.Images.FirstOrDefault();
            }

            if (categoryDto.Properties != null)
            {
                result.Properties = categoryDto.Properties
                    .Where(x => string.Equals(x.Type, "Category", StringComparison.OrdinalIgnoreCase))
                    .Select(p => ToProperty(p, currentLanguage))
                    .ToList();
            }

            return result;
        }

        public static Image ToImage(this catalogDto.Image imageDto)
        {
            var result = new Image
            {
                Url = imageDto.Url.RemoveLeadingUriScheme()
            };

            return result;
        }

        public static Asset ToAsset(this catalogDto.Asset assetDto)
        {
            var result = new Asset
            {
                Url = assetDto.Url.RemoveLeadingUriScheme(),
                TypeId = assetDto.TypeId,
                Size = assetDto.Size,
                Name = assetDto.Name,
                MimeType = assetDto.MimeType,
                Group = assetDto.Group
            };

            return result;
        }

        public static Product ToProduct(this catalogDto.Product productDto, Language currentLanguage, Currency currentCurrency, Store store)
        {
            var result = new Product(currentCurrency, currentLanguage)
            {
                Id = productDto.Id,
                CatalogId = productDto.CatalogId,
                CategoryId = productDto.CategoryId,
                DownloadExpiration = productDto.DownloadExpiration,
                DownloadType = productDto.DownloadType,
                EnableReview = productDto.EnableReview ?? false,
                Gtin = productDto.Gtin,
                HasUserAgreement = productDto.HasUserAgreement ?? false,
                IsActive = productDto.IsActive ?? false,
                IsBuyable = productDto.IsBuyable ?? false,
                ManufacturerPartNumber = productDto.ManufacturerPartNumber,
                MaxNumberOfDownload = productDto.MaxNumberOfDownload ?? 0,
                MaxQuantity = productDto.MaxQuantity ?? 0,
                MeasureUnit = productDto.MeasureUnit,
                MinQuantity = productDto.MinQuantity ?? 0,
                Name = productDto.Name,
                PackageType = productDto.PackageType,
                ProductType = productDto.ProductType,
                ShippingType = productDto.ShippingType,
                TaxType = productDto.TaxType,
                TrackInventory = productDto.TrackInventory ?? false,
                VendorId = productDto.Vendor,
                WeightUnit = productDto.WeightUnit,
                Weight = (decimal?)productDto.Weight,
                Height = (decimal?)productDto.Height,
                Width = (decimal?)productDto.Width,
                Length = (decimal?)productDto.Length,
                Sku = productDto.Code,
                Outline = productDto.Outlines.GetOutlinePath(store.Catalog),
                SeoPath = productDto.Outlines.GetSeoPath(store, currentLanguage, null),
            };
            result.Url = "~/" + (result.SeoPath ?? "product/" + result.Id);

            if (productDto.Properties != null)
            {
                result.Properties = productDto.Properties
                    .Where(x => string.Equals(x.Type, "Product", StringComparison.InvariantCultureIgnoreCase))
                    .Select(p => ToProperty(p, currentLanguage))
                    .ToList();

                result.VariationProperties = productDto.Properties
                    .Where(x => string.Equals(x.Type, "Variation", StringComparison.InvariantCultureIgnoreCase))
                    .Select(p => ToProperty(p, currentLanguage))
                    .ToList();
            }

            if (productDto.Images != null)
            {
                result.Images = productDto.Images.Select(ToImage).ToArray();
                result.PrimaryImage = result.Images.FirstOrDefault();
            }

            if (productDto.Assets != null)
            {
                result.Assets = productDto.Assets.Select(ToAsset).ToList();
            }

            if (productDto.Variations != null)
            {
                result.Variations = productDto.Variations.Select(v => ToProduct(v, currentLanguage, currentCurrency, store)).ToList();
            }

            if (!productDto.Associations.IsNullOrEmpty())
            {
                result.Associations.AddRange(productDto.Associations.Select(ToAssociation).Where(x => x != null));
            }

            if (!productDto.SeoInfos.IsNullOrEmpty())
            {
                var seoInfoDto = productDto.SeoInfos.Select(x => x.JsonConvert<coreDto.SeoInfo>())
                    .GetBestMatchingSeoInfos(store, currentLanguage)
                    .FirstOrDefault();

                if (seoInfoDto != null)
                {
                    result.SeoInfo = seoInfoDto.ToSeoInfo();
                }
            }

            if (result.SeoInfo == null)
            {
                result.SeoInfo = new SeoInfo
                {
                    Title = productDto.Id,
                    Language = currentLanguage,
                    Slug = productDto.Code
                };
            }

            if (productDto.Reviews != null)
            {
                result.Descriptions = productDto.Reviews.Where(r => !string.IsNullOrEmpty(r.Content)).Select(r => new EditorialReview
                {
                    Language = new Language(r.LanguageCode),
                    ReviewType = r.ReviewType,
                    Value = Markdown.ToHtml(r.Content, _markdownPipeline)
                }).Where(x => x.Language.Equals(currentLanguage)).ToList();
                result.Description = result.Descriptions.FindWithLanguage(currentLanguage, x => x.Value, null);
            }

            return result;
        }


        public static marketingDto.ProductPromoEntry ToProductPromoEntryDto(this Product product)
        {
            var result = new marketingDto.ProductPromoEntry
            {
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Outline = product.Outline,
                ProductId = product.Id,
                Quantity = 1,
                InStockQuantity = product.Inventory != null && product.Inventory.InStockQuantity.HasValue ? (int)product.Inventory.InStockQuantity.Value : 0,
                Variations = product.Variations?.Select(ToProductPromoEntryDto).ToList()
            };

            if (product.Price != null)
            {
                result.Discount = (double)product.Price.DiscountAmount.Amount;
                result.Price = (double)product.Price.SalePrice.Amount;
            }

            return result;
        }


        public static TaxLine[] ToTaxLines(this Product product)
        {
            var result = new List<TaxLine>
            {
                new TaxLine(product.Currency)
                {
                    Id = product.Id,
                    Code = product.Sku,
                    Name = product.Name,
                    TaxType = product.TaxType,
                    //Special case when product have 100% discount and need to calculate tax for old value
                    Amount =  product.Price.ActualPrice.Amount > 0 ? product.Price.ActualPrice : product.Price.SalePrice
                }
            };

            //Need generate tax line for each tier price
            foreach (var tierPrice in product.Price.TierPrices)
            {
                result.Add(new TaxLine(tierPrice.Price.Currency)
                {
                    Id = product.Id,
                    Code = product.Sku,
                    Name = product.Name,
                    TaxType = product.TaxType,
                    Quantity = (int)tierPrice.Quantity,
                    Amount = tierPrice.Price
                });
            }

            return result.ToArray();
        }
    }
}
