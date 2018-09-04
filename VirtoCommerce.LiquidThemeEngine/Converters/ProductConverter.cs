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
            var result = new Product();
            result.Id = product.Id;
            result.CatalogId = product.CatalogId;
            result.CategoryId = product.CategoryId;
            result.Description = product.Description;
            result.IsQuotable = product.IsQuotable;
            result.TaxType = product.TaxType;

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

            result.FeaturedImage = product.PrimaryImage != null ? product.PrimaryImage.ToShopifyModel() : null;

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

            if (!product.Associations.IsNullOrEmpty())
            {
                result.RelatedProducts = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    //Need to load related products from associated  product and categories
                    var skip = (pageNumber - 1) * pageSize;
                    var take = pageSize;
                    var productAssociations = product.Associations.OfType<storefrontModel.ProductAssociation>().OrderBy(x => x.Priority);
                    var retVal = productAssociations.Select(x => x.Product).Skip(skip).Take(take).ToList();
                    var totalCount = productAssociations.Count();
                    skip = Math.Max(0, skip - totalCount);
                    take = Math.Max(0, take - retVal.Count);

                    //Load product from associated categories with correct pagination
                    foreach (var categoryAssociation in product.Associations.OfType<storefrontModel.CategoryAssociation>().OrderBy(x => x.Priority))
                    {
                        if (categoryAssociation.Category != null && categoryAssociation.Category.Products != null)
                        {
                            categoryAssociation.Category.Products.Slice(skip / pageSize + 1, take, sortInfos);
                            retVal.AddRange(categoryAssociation.Category.Products);
                            totalCount += categoryAssociation.Category.Products.GetTotalCount();
                            skip = Math.Max(0, skip - totalCount);
                            take = Math.Max(0, take - categoryAssociation.Category.Products.Count());
                        }
                    }

                    return new StaticPagedList<Product>(retVal.Select(x => x.ToShopifyModel()), pageNumber, pageSize, totalCount);
                }, 1, 20);
            }

            if (product.Vendor != null)
            {
                result.Vendor = product.Vendor.ToShopifyModel();
            }
            return result;
        }

        public virtual Variant ToLiquidVariant(storefrontModel.Product product)
        {
            var result = new Variant();

            result.Available = product.IsAvailable;
            result.Buyable = product.IsBuyable;
            result.InStock = product.IsInStock;
            result.Barcode = product.Gtin;
            result.CatalogId = product.CatalogId;
            result.CategoryId = product.CategoryId;
            result.Id = product.Id;
            result.InventoryPolicy = product.TrackInventory ? "deny" : "continue";
            result.InventoryQuantity = product.Inventory != null ? Math.Max(0, (product.Inventory.InStockQuantity ?? 0L) - (product.Inventory.ReservedQuantity ?? 0L)) : 0;
            result.Options = product.VariationProperties.Where(p => !string.IsNullOrEmpty(p.Value)).Select(p => p.Value).ToArray();
            result.CompareAtPrice = product.Price.ListPrice.Amount * 100;
            result.CompareAtPriceWithTax = product.Price.ListPriceWithTax.Amount * 100;
            result.Price = product.Price.ActualPrice.Amount * 100;
            result.PriceWithTax = product.Price.ActualPriceWithTax.Amount * 100;
            result.Selected = false;
            result.Sku = product.Sku;
            result.Title = product.Name;
            result.Url = product.Url;
            result.Weight = product.Weight ?? 0m;
            result.WeightUnit = product.WeightUnit;
            result.FeaturedImage = product.PrimaryImage != null ? product.PrimaryImage.ToShopifyModel() : null;

            result.PackageType = product.PackageType;
            result.Height = product.Height;
            result.MeasureUnit = product.MeasureUnit;
            result.Width = product.Width;
            result.Length = product.Length;



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
