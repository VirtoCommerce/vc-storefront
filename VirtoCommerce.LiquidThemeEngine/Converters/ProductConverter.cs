using PagedList.Core;
using System;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using storefrontModel = VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ProductStaticConverter
    {
        public static Product ToShopifyModel(this storefrontModel.Product product)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidProduct(product);
        }

        public static Variant ToVariant(this storefrontModel.Product product)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidVariant(product);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Product ToLiquidProduct(storefrontModel.Product product)
        {
            var result = new Product
            {
                Id = product.Id,
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Description = product.Description,
                IsQuotable = product.IsQuotable,
                TaxType = product.TaxType
            };

            result.Variants.Add(ToLiquidVariant(product));

            if (product.Variations != null && product.Variations.Any())
            {
                result.Variants.AddRange(product.Variations.Select(x => x.ToVariant()));
                result.Available = product.IsAvailable || product.Variations.Any(v => v.IsAvailable);
                result.Buyable = product.IsBuyable || product.Variations.Any(v => v.IsBuyable);
                result.InStock = product.IsInStock || product.Variations.Any(v => v.IsInStock);
            }
            else
            {
                result.Available = product.IsAvailable;
                result.Buyable = product.IsBuyable;
                result.InStock = product.IsInStock;
            }

            result.CatalogId = product.CatalogId;
            result.CategoryId = product.CategoryId;

            result.CompareAtPriceMax = result.Variants.Select(x => x.CompareAtPrice).Max();
            result.CompareAtPriceMin = result.Variants.Select(x => x.CompareAtPrice).Min();
            result.CompareAtPriceVaries = result.CompareAtPriceMax != result.CompareAtPriceMin;

            result.CompareAtPrice = product.Price.ListPrice.Amount * 100;
            result.CompareAtPriceWithTax = product.Price.ListPriceWithTax.Amount * 100;
            result.Price = product.Price.ActualPrice.Amount * 100;
            result.PriceWithTax = product.Price.ActualPriceWithTax.Amount * 100;

            result.PriceMax = result.Variants.Select(x => x.Price).Max();
            result.PriceMin = result.Variants.Select(x => x.Price).Min();
            result.PriceVaries = result.PriceMax != result.PriceMin;

            result.Content = product.Description;
            result.Description = result.Content;

            result.Descriptions = new Descriptions(product.Descriptions.Select(d => new Description
            {
                Content = d.Value,
                Type = d.ReviewType
            }));

            result.FeaturedImage = product.PrimaryImage?.ToShopifyModel();

            if (result.FeaturedImage != null)
            {
                result.FeaturedImage.ProductId = product.Id;
                result.FeaturedImage.AttachedToVariant = false;
            }

            result.FirstAvailableVariant = result.Variants.FirstOrDefault(x => x.Available);
            result.Handle = product.SeoInfo != null ? product.SeoInfo.Slug : product.Id;
            result.Images = product.Images.Select(x => x.ToShopifyModel()).ToArray();

            foreach (var image in result.Images)
            {
                image.ProductId = product.Id;
                image.AttachedToVariant = false;
            }

            if (product.VariationProperties != null)
            {
                result.Options = product.VariationProperties.Where(x => !string.IsNullOrEmpty(x.Value)).Select(x => x.Name).ToArray();
            }

            if (product.Properties != null)
            {
                result.Properties = product.Properties.Select(x => x.ToShopifyModel()).ToList();
                result.Metafields = new MetaFieldNamespacesCollection(new[] { new MetafieldsCollection("properties", product.Properties) });
            }

            result.SelectedVariant = result.Variants.First();
            result.Title = product.Name;
            result.Type = product.ProductType;
            result.Url = product.Url;

            result.PackageType = product.PackageType;
            result.WeightUnit = product.WeightUnit;
            result.Weight = product.Weight;
            result.Height = product.Height;
            result.MeasureUnit = product.MeasureUnit;
            result.Width = product.Width;
            result.Length = product.Length;
            result.Outline = product.Outline;
            result.AvailableQuantity = product.AvailableQuantity;

            if (product.Associations != null)
            {
                result.RelatedProducts = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    product.Associations.Slice(pageNumber, pageSize, sortInfos, @params);
                    return new StaticPagedList<Product>(product.Associations.Where(x => x.Product != null).Select(x => x.Product.ToShopifyModel()), product.Associations);
                }, product.Associations.PageNumber, product.Associations.PageSize);
            }

            if (product.Vendor != null)
            {
                result.Vendor = product.Vendor.ToShopifyModel();
            }
            return result;
        }

        public virtual Variant ToLiquidVariant(storefrontModel.Product product)
        {
            var result = new Variant
            {
                Available = product.IsAvailable,
                Buyable = product.IsBuyable,
                InStock = product.IsInStock,
                Barcode = product.Gtin,
                CatalogId = product.CatalogId,
                CategoryId = product.CategoryId,
                Id = product.Id,
                InventoryPolicy = product.TrackInventory ? "deny" : "continue",
                InventoryQuantity = product.Inventory != null ? Math.Max(0, (product.Inventory.InStockQuantity ?? 0L) - (product.Inventory.ReservedQuantity ?? 0L)) : 0,
                Options = product.VariationProperties.Where(p => !string.IsNullOrEmpty(p.Value)).Select(p => p.Value).ToArray(),
                CompareAtPrice = product.Price.ListPrice.Amount * 100,
                CompareAtPriceWithTax = product.Price.ListPriceWithTax.Amount * 100,
                Price = product.Price.ActualPrice.Amount * 100,
                PriceWithTax = product.Price.ActualPriceWithTax.Amount * 100,
                Selected = false,
                Sku = product.Sku,
                Title = product.Name,
                Url = product.Url,
                Weight = product.Weight ?? 0m,
                WeightUnit = product.WeightUnit,
                FeaturedImage = product.PrimaryImage?.ToShopifyModel(),

                PackageType = product.PackageType,
                Height = product.Height,
                MeasureUnit = product.MeasureUnit,
                Width = product.Width,
                Length = product.Length,
                AvailableQuantity = product.AvailableQuantity
            };



            if (result.FeaturedImage != null)
            {
                result.FeaturedImage.ProductId = product.Id;
                result.FeaturedImage.AttachedToVariant = true;
                result.FeaturedImage.Variants = new[] { result };
            }

            return result;
        }
    }
}
