using System;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Catalog.Specifications;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Contracts.Catalog;
using VirtoCommerce.Storefront.Model.Inventory;
using VirtoCommerce.Storefront.Model.Stores;
using catalogDto = VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using marketingDto = VirtoCommerce.Storefront.AutoRestClients.MarketingModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static partial class CatalogConverter
    {
        private static MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    
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

            var aggrItemIsVisbileSpec = new AggregationItemIsVisibleSpecification();
            if (aggregationDto.Items != null)
            {
                result.Items = aggregationDto.Items.Select(i => i.ToAggregationItem(result, currentLanguage))
                                                   .Where(x => aggrItemIsVisbileSpec.IsSatisfiedBy(x))
                                                   .Distinct().ToArray();
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

        public static AggregationItem ToAggregationItem(this catalogDto.AggregationItem itemDto, Aggregation aggregationGroup, string currentLanguage)
        {
            var result = new AggregationItem
            {
                Group = aggregationGroup,
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

            if (aggregationGroup.Field.EqualsInvariant("__outline"))
            {
                result = CategoryAggregationItem.FromAggregationItem(result);
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
                IsMultivalue = propertyDto.Multivalue ?? false,
                Hidden = propertyDto.Hidden ?? false
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
            if ((propertyDto.Multilanguage ?? false) && propertyDto.Values != null)
            {
                result.LocalizedValues = propertyDto.Values.Where(x => x.Value != null).Select(x => new LocalizedString(new Language(x.LanguageCode), x.Value.ToString())).ToList();
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

        public static catalogDto.ProductIndexedSearchCriteria ToProductSearchCriteriaDto(this ProductSearchCriteria criteria, WorkContext workContext)
        {
            var currency = criteria.Currency ?? workContext.CurrentCurrency;

            var result = new catalogDto.ProductIndexedSearchCriteria
            {
                SearchPhrase = criteria.Keyword,
                LanguageCode = criteria.Language?.CultureName ?? workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id,
                CatalogId = workContext.CurrentStore.Catalog,
                Outline = criteria.Outline,
                Currency = currency.Code,
                Pricelists = workContext.CurrentPricelists.Where(p => p.Currency.Equals(currency)).Select(p => p.Id).ToList(),
                PriceRange = criteria.PriceRange?.ToNumericRangeDto(),
                UserGroups = criteria.UserGroups,
                Terms = criteria.Terms.ToStrings(),
                Sort = criteria.SortBy,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                ResponseGroup = ((int)criteria.ResponseGroup).ToString(),
                IsFuzzySearch = criteria.IsFuzzySearch,
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

        public static catalogDto.CategoryIndexedSearchCriteria ToCategorySearchCriteriaDto(this CategorySearchCriteria criteria, WorkContext workContext)
        {
            var result = new catalogDto.CategoryIndexedSearchCriteria
            {
                SearchPhrase = criteria.Keyword,
                LanguageCode = criteria.Language?.CultureName ?? workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id,
                CatalogId = workContext.CurrentStore.Catalog,
                Outline = criteria.Outline,
                UserGroups = workContext.CurrentUser?.Contact?.UserGroups ?? new List<string>(), // null value disables filtering by user groups
                Sort = criteria.SortBy,
                Skip = criteria.Start,
                Take = criteria.PageSize,
                ResponseGroup = ((int)criteria.ResponseGroup).ToString(),
                IsFuzzySearch = criteria.IsFuzzySearch,
            };

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
            if (result.Outline != null)
            {
                //Need to take virtual parent from outline (get second last) because for virtual catalog category.ParentId still points to a physical category
                result.ParentId = result.Outline.Split("/").Reverse().Skip(1).Take(1).FirstOrDefault() ?? result.ParentId;
            }
            result.Url = "/" + (result.SeoPath ?? "category/" + categoryDto.Id);

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
                result.Properties = new MutablePagedList<CatalogProperty>(categoryDto.Properties
                    .Where(x => string.Equals(x.Type, "Category", StringComparison.OrdinalIgnoreCase))
                    .Select(p => ToProperty(p, currentLanguage)));
            }

            return result;
        }

        public static Image ToImage(this catalogDto.Image imageDto)
        {
            var result = new Image
            {
                Url = imageDto.Url.RemoveLeadingUriScheme(),
                SortOrder = imageDto.SortOrder,
                Group = imageDto.Group,
                LanguageCode = imageDto.LanguageCode
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

        public static Product ToProduct(this catalogDto.Variation variationDto, Language currentLanguage, Currency currentCurrency, Store store)
        {
            return variationDto.JsonConvert<catalogDto.CatalogProduct>().ToProduct(currentLanguage, currentCurrency, store);
        }

        public static Product ToProduct(this catalogDto.CatalogProduct productDto, Language currentLanguage, Currency currentCurrency, Store store)
        {
            var result = new Product(currentCurrency, currentLanguage)
            {
                Id = productDto.Id,
                TitularItemId = productDto.TitularItemId,
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
            result.Url = "/" + (result.SeoPath ?? "product/" + result.Id);

            if (productDto.Properties != null)
            {
                result.Properties = new MutablePagedList<CatalogProperty>(productDto.Properties
                    .Where(x => string.Equals(x.Type, "Product", StringComparison.InvariantCultureIgnoreCase))
                    .Select(p => ToProperty(p, currentLanguage))
                    .ToList());

                if (productDto.IsActive.GetValueOrDefault())
                {
                    result.VariationProperties = new MutablePagedList<CatalogProperty>(productDto.Properties
                        .Where(x => string.Equals(x.Type, "Variation", StringComparison.InvariantCultureIgnoreCase))
                        .Select(p => ToProperty(p, currentLanguage))
                        .ToList());
                }
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
                // Reviews for currentLanguage (or Invariant language as fall-back) for each ReviewType
                var descriptions = productDto.Reviews
                                        .Where(r => !string.IsNullOrEmpty(r.Content))
                                        .Select(r => new EditorialReview
                                        {
                                            Language = new Language(r.LanguageCode),
                                            ReviewType = r.ReviewType,
                                            Value = Markdown.ToHtml(r.Content, _markdownPipeline)
                                        });
                //Select only best matched description for current language in the each description type
                var tmpDescriptionList = new List<EditorialReview>();
                foreach (var descriptionGroup in descriptions.GroupBy(x => x.ReviewType))
                {
                    var description = descriptionGroup.FindWithLanguage(currentLanguage);
                    if (description != null)
                    {
                        tmpDescriptionList.Add(description);
                    }
                }
                result.Descriptions = new MutablePagedList<EditorialReview>(tmpDescriptionList);
                result.Description = (result.Descriptions.FirstOrDefault(x => x.ReviewType.EqualsInvariant("FullReview")) ?? result.Descriptions.FirstOrDefault())?.Value;
            }


            return result;
        }

        public static Product[] ToProducts(this ProductDto[] productDtos, WorkContext workContext)
        {
            var result = productDtos.Select(x => x.ToProduct(workContext));

            return result.ToArray();
        }

        public static Product ToProduct(this ProductDto productDto, WorkContext workContext)
        {
            var result = new Product(workContext.CurrentCurrency, workContext.CurrentLanguage)
            {
                Id = productDto.Id,
                Name = productDto.Name,
                CategoryId = productDto.Category?.Id,
                Sku = productDto.Code,
                Description = productDto.Descriptions?.FirstOrDefault(d => d.ReviewType.EqualsInvariant("FullReview"))?.Content,
                CatalogId = productDto.CatalogId,
                SeoPath = productDto?.Outlines.GetSeoPath(workContext.CurrentStore, workContext.CurrentLanguage, null),
                IsAvailable = productDto?.AvailabilityData?.IsAvailable ?? false,
                IsBuyable = productDto?.AvailabilityData?.IsBuyable ?? false,
                IsInStock = productDto?.AvailabilityData?.IsInStock ?? false,
                //Height = decimal.MinValue, // TBD
                //Length = decimal.MinValue, // TBD
                //MeasureUnit = "", // TBD
                Outline = productDto?.Outlines.GetOutlinePath(workContext.CurrentStore.Catalog),
                ProductType = productDto.ProductType,
                //TaxType = "", // TBD
                //Weight = 0, // TBD
                //WeightUnit = "", // TBD
                //Width = 0, // TBD
            };

            result.Url = "/" + (result.SeoPath ?? "product/" + result.Id);

            if (!productDto?.Assets.IsNullOrEmpty() ?? false)
            {
                result.Assets.AddRange(productDto?.Assets?.Select(ToAsset));
            }

            if (!productDto?.Images.IsNullOrEmpty() ?? false)
            {
                result.Images.AddRange(productDto.Images.Select(ToImage));

                result.PrimaryImage = result.Images.FirstOrDefault();
            }

            if (!productDto.Associations?.Items.IsNullOrEmpty() ?? false)
            {
                result.Associations = new MutablePagedList<ProductAssociation>(productDto.Associations.Items.Select(i =>
                {
                    var association = new ProductAssociation
                    {
                        Priority = i.Priority,
                        Product = new Product { Id = i.Product?.Id },
                        ProductId = productDto.Id,
                        Quantity = i.Quantity,
                        Tags = i.Tags,
                        Type = i.Type,
                    };

                    return association;
                }));
            }

            if (!productDto.Descriptions.IsNullOrEmpty())
            {
                result.Descriptions = new MutablePagedList<EditorialReview>(productDto.Descriptions?.Select(d =>
                    new EditorialReview
                    {
                        Value = d.Content,
                        ReviewType = d.ReviewType,
                        Language = new Language(d.LanguageCode),
                    })
                );
            }

            if (!productDto?.AvailabilityData?.Inventories.IsNullOrEmpty() ?? false)
            {
                result.InventoryAll = productDto?.AvailabilityData?.Inventories.Select(x =>
                {
                    var inventory = new Inventory
                    {
                        AllowBackorder = x.AllowBackorder,
                        AllowPreorder = x.AllowPreorder,
                        BackorderAvailabilityDate = x.BackorderAvailableDate,
                        FulfillmentCenterId = x.FulfillmentCenterId,
                        InStockQuantity = x.InStockQuantity,
                        PreorderAvailabilityDate = x.PreorderAvailabilityDate,
                        ProductId = productDto.Id,
                        ReservedQuantity = x.ReservedQuantity,
                    };

                    return inventory;
                }).ToArray();

                result.Inventory = workContext.CurrentStore.DefaultFulfillmentCenterId != null ?
                    result.InventoryAll.FirstOrDefault(x => x.FulfillmentCenterId == workContext.CurrentStore.DefaultFulfillmentCenterId)
                    : result.InventoryAll.FirstOrDefault();
            }

            if (!productDto.Variations?.IsNullOrEmpty() ?? false)
            {
                result.Variations.AddRange(productDto.Variations.Select(x =>
                {
                    var product = new Product(workContext.CurrentCurrency, workContext.CurrentLanguage)
                    {
                        Id = x.Id,
                        Sku = x.Code,
                        Images = x.Images?.Select(ToImage).ToArray(),
                        Assets = x.Assets?.Select(ToAsset).ToArray(),
                    };

                    if (!x.Prices.IsNullOrEmpty())
                    {
                        var productPrices = x.Prices.ToPrices(workContext.AllCurrencies, productDto.Tax?.Rates);

                        product.ApplyPrices(productPrices, workContext.CurrentCurrency, workContext.AllCurrencies);
                    }

                    return product;

                }));
            }

            if (!productDto?.Properties.IsNullOrEmpty() ?? false)
            {
                result.Properties = new MutablePagedList<CatalogProperty>(productDto?.Properties.GroupBy(x => x.Id).Select(
                    x =>
                    {
                        var propertyValues = x.Select(p => p.Value);
                        var propertyDto = x.First();

                        var property = new CatalogProperty
                        {
                            Id = propertyDto.Id,
                            Name = propertyDto.Name,
                            Value = propertyDto.Value,
                            ValueType = propertyDto.ValueType,
                            ValueId = propertyDto.ValueId,
                            DisplayName = propertyDto.Label,
                            Hidden = propertyDto.Hidden,
                            Type = propertyDto.ValueType,
                            Values = propertyValues.ToArray(),
                            LocalizedValues = new List<LocalizedString>{ new LocalizedString(workContext.CurrentLanguage, propertyDto.Label) },
                        };

                        return property;
                    }));
            }

            if (!productDto?.Prices.FirstOrDefault()?.Discounts.IsNullOrEmpty() ?? false)
            {
                var discountsCollection = productDto?.Prices.SelectMany(x =>
                {
                    var discounts = x.Discounts.Select(d =>
                    {
                        var currency = new Currency(workContext.CurrentLanguage, x.Currency);

                        var discount = new Model.Marketing.Discount
                        {
                            Amount = new Money((double?)d.Amount.Amount ?? 0d, currency),
                            Coupon = d.Coupon,
                            Description = d.Description,
                            PromotionId = d.PromotionId,
                        };

                        return discount;
                    });

                    return discounts;
                }).ToArray();

                if (!discountsCollection.IsNullOrEmpty())
                {
                    result.Discounts.AddRange(discountsCollection);
                }
            }

            if (!productDto.SeoInfos.IsNullOrEmpty())
            {
                var seoInfoDto = productDto.SeoInfos.Select(x => x.JsonConvert<SeoInfoDto>())
                    .GetBestMatchingSeoInfos(workContext.CurrentStore, workContext.CurrentLanguage)
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
                    Language = workContext.CurrentLanguage,
                    Slug = productDto.Code
                };
            }

            if (!productDto?.Prices.IsNullOrEmpty() ?? false)
            {

                var productPrices = productDto.Prices.ToPrices(workContext.AllCurrencies, productDto.Tax?.Rates);

                result.ApplyPrices(productPrices, workContext.CurrentCurrency, workContext.AllCurrencies);
            }

            return result;
        }

        public static ProductPrice[] ToPrices(this PriceDto[] priceDtos, IList<Currency> currencies, TaxRateDto[] taxRateDtos)
        {
            var result = priceDtos.Select(x =>
            {
                var currency = currencies.FirstOrDefault(c => c.Code.EqualsInvariant(x.Currency));

                var price = new ProductPrice(currency)
                {
                    ListPrice = new Money((double?)x.List.Amount ?? 0d, currency),
                    PricelistId = x.PricelistId,
                    MinQuantity = x.MinQuantity,
                    DiscountAmount = new Money((double?)x.DiscountAmount.Amount ?? 0d, currency),
                    SalePrice = new Money((double?)x.Sale.Amount ?? 0d, currency),
                    TierPrices = x.TierPrices.Select(t => new TierPrice(new Money((double?)t.Price.Amount ?? 0d, currency), t.Quantity ?? 0)).ToList(),
                };

                if (!taxRateDtos.IsNullOrEmpty())
                {
                    var taxRates = taxRateDtos.Select(t =>
                    {
                        var rate = new TaxRate(currency)
                        {
                            Line = new TaxLine(currency)
                            {
                                Amount = new Money((double?)t.Line.Amount ?? 0d, currency),
                                Code = t.Line.Code,
                                Id = t.Line.Id,
                                Name = t.Line.Name,
                                Quantity = t.Line.Quantity ?? 0,
                                TaxType = t.Line.TaxType,
                                Price = new Money((double?)t.Line.Price ?? 0d, currency),
                            },
                            Rate = new Money((double?)t.Rate ?? 0d, currency),
                            PercentRate = t.PercentRate ?? 0m,
                        };

                        return rate;
                    });

                    price.ApplyTaxRates(taxRates);
                }

                return price;
            });

            return result.ToArray();
        }

        public static TaxRate[] ToTaxRates(this TaxRateDto[] taxRateDtos, Currency currency)
        {
            var result = taxRateDtos.Select(x =>
            {
                var rate = new TaxRate(currency)
                {
                    Line = new TaxLine(currency)
                    {
                        Amount = new Money((double?)x.Line.Amount ?? 0d, currency),
                        Code = x.Line.Code,
                        Id = x.Line.Id,
                        Name = x.Line.Name,
                        Quantity = x.Line.Quantity ?? 0,
                        TaxType = x.Line.TaxType,
                        Price = new Money((double?)x.Line.Price ?? 0d, currency),
                    },
                    Rate = new Money((double?)x.Rate ?? 0d, currency),
                    PercentRate = x.PercentRate ?? 0m,
                };

                return rate;
            });

            return result.ToArray();
        }

        public static Category[] ToCategories(this CategoryDto[] categoryDtos, Store store, Language language)
        {
            var result = categoryDtos.Select(x => x.ToCategory(store, language));

            return result.ToArray();
        }

        public static Category ToCategory(this CategoryDto categoryDto, Store store, Language language)
        {
            var result = new Category
            {
                Id = categoryDto.Id,
                Code = categoryDto.Code,
                Name = categoryDto.Name,
                ParentId = categoryDto.Parent?.Id,
                SeoPath = categoryDto.Outlines.GetSeoPath(store, language, null),
                Outline = categoryDto.Outlines.GetOutlinePath(store.Catalog),
            };

            if (result.Outline != null)
            {
                result.ParentId = result.Outline.Split("/").Reverse().Skip(1).Take(1).FirstOrDefault() ?? result.ParentId;
            }

            result.Url = "/" + (result.SeoPath ?? "category/" + categoryDto.Id);

            if (!categoryDto.SeoInfos.IsNullOrEmpty())
            {
                var seoInfoDto = categoryDto
                    .SeoInfos
                    .Select(x => x.JsonConvert<SeoInfoDto>())
                    .GetBestMatchingSeoInfos(store, language)
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
                    Title = categoryDto.Name,
                };
            }

            if (!categoryDto.Images.IsNullOrEmpty())
            {
                result.Images = categoryDto.Images.Select(ToImage).ToArray();
                result.PrimaryImage = result.Images.FirstOrDefault();
            }

            return result;
        }

        public static Image ToImage(this ImageDto imageDto)
        {
            var result = new Image
            {
                Alt = imageDto.Name, //+
                FullSizeImageUrl = imageDto.Url, //+
                Group = imageDto.Group,
                SortOrder = imageDto.SortOrder,
                Title = imageDto.Name,
                Url = imageDto.Url,
            };

            return result;
        }

        public static Asset ToAsset(this AssetDto assetDto)
        {
            var result = new Asset
            {
                MimeType = assetDto.MimeType,
                Name = assetDto.Name,
                Size = assetDto.Size,
                Group = assetDto.Group,
                Url = assetDto.Url,
                TypeId = assetDto.TypeId,
            };

            return result;
        }

        public static marketingDto.ProductPromoEntry ToProductPromoEntryDto(this Product product)
        {
            var result = new marketingDto.ProductPromoEntry
            {
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Outline = product.Outline,
                Code = product.Sku,
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

        public static catalogDto.ProductAssociationSearchCriteria ToProductAssociationSearchCriteriaDto(this ProductAssociationSearchCriteria criteria)
        {
            var result = new catalogDto.ProductAssociationSearchCriteria
            {
                Group = criteria.Group,
                ObjectIds = new string[] { criteria.ProductId },
                ResponseGroup = criteria.ResponseGroup.ToString(),
                Skip = criteria.Start,
                Take = criteria.PageSize
            };
            return result;
        }

        public static ProductAssociation ToProductAssociation(this catalogDto.ProductAssociation associationDto)
        {
            var result = new ProductAssociation
            {
                Type = associationDto.Type,
                ProductId = associationDto.AssociatedObjectId,
                Priority = associationDto.Priority ?? 0,
                Quantity = associationDto.Quantity,
                Tags = associationDto.Tags
            };
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

        public static Aggregation ToAggregation(this TermFacet termFacet, string currentLanguage)
        {
            var result = new Aggregation
            {
                AggregationType = "term",
                Field = termFacet.Name
            };

            var aggrItemIsVisbileSpec = new AggregationItemIsVisibleSpecification();
            if (termFacet.Terms != null)
            {
                result.Items = termFacet.Terms.Select(i => i.ToAggregationItem(result, currentLanguage))
                                                   .Where(x => aggrItemIsVisbileSpec.IsSatisfiedBy(x))
                                                   .Distinct().ToArray();
            }
            //TODO:
            //if (termFacet.Labels != null)
            //{
            //    result.Label =
            //        aggregationDto.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
            //            .Select(l => l.Label)
            //            .FirstOrDefault();
            //}

            if (string.IsNullOrEmpty(result.Label))
            {
                result.Label = termFacet.Name;
            }

            return result;
        }

        public static Aggregation ToAggregation(this RangeFacet rangeFacet, string currentLanguage)
        {
            var result = new Aggregation
            {
                AggregationType = "range",
                Field = rangeFacet.Name
            };


            var aggrItemIsVisbileSpec = new AggregationItemIsVisibleSpecification();
            if (rangeFacet.Ranges != null)
            {
                result.Items = rangeFacet.Ranges.Select(i => i.ToAggregationItem(result, currentLanguage))
                                                   .Where(x => aggrItemIsVisbileSpec.IsSatisfiedBy(x))
                                                   .Distinct().ToArray();
            }
            //TODO:
            //if (aggregationDto.Labels != null)
            //{
            //    result.Label =
            //        aggregationDto.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
            //            .Select(l => l.Label)
            //            .FirstOrDefault();
            //}

            if (string.IsNullOrEmpty(result.Label))
            {
                result.Label = rangeFacet.Name;
            }

            return result;
        }

        public static AggregationItem ToAggregationItem(this FacetTermDto termDto, Aggregation aggregationGroup, string currentLanguage)
        {
            var result = new AggregationItem
            {
                Group = aggregationGroup,
                Value = termDto.Term,
                IsApplied = termDto.IsSelected ?? false,
                Count = (int)(termDto.Count ?? 0),            
            };

            //TODO:
            //if (itemDto.Labels != null)
            //{
            //    result.Label =
            //        itemDto.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
            //            .Select(l => l.Label)
            //            .FirstOrDefault();
            //}

            if (string.IsNullOrEmpty(result.Label) && termDto.Term != null)
            {
                result.Label = termDto.Term.ToString();
            }

            if (aggregationGroup.Field.EqualsInvariant("__outline"))
            {
                result = CategoryAggregationItem.FromAggregationItem(result);
            }

            return result;
        }
        public static AggregationItem ToAggregationItem(this FacetRangeTypeDto itemDto, Aggregation aggregationGroup, string currentLanguage)
        {
            var result = new AggregationItem
            {
                Group = aggregationGroup,
                Value = aggregationGroup.Field,
                IsApplied = itemDto.IsSelected ?? false,
                Count = (int)(itemDto.Count ?? 0),
                Lower = itemDto.From?.ToString(),
                Upper = itemDto.To?.ToString(),
            };

            //if (itemDto.Labels != null)
            //{
            //    result.Label =
            //        itemDto.Labels.Where(l => string.Equals(l.Language, currentLanguage, StringComparison.OrdinalIgnoreCase))
            //            .Select(l => l.Label)
            //            .FirstOrDefault();
            //}

            if (string.IsNullOrEmpty(result.Label) && aggregationGroup.Field != null)
            {
                result.Label = aggregationGroup.Field.ToString();
            }

            if (aggregationGroup.Field.EqualsInvariant("__outline"))
            {
                result = CategoryAggregationItem.FromAggregationItem(result);
            }

            return result;
        }
    }
}
