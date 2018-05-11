using PagedList.Core;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class VendorConverter
    {
        public static Vendor ToShopifyModel(this StorefrontModel.Vendor vendor)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidVendor(vendor);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Vendor ToLiquidVendor(Storefront.Model.Vendor vendor)
        {
            var result = new Vendor();
            result.Description = vendor.Description;
            result.GroupName = vendor.GroupName;
            result.Id = vendor.Id;
            result.LogoUrl = vendor.LogoUrl;
            result.Name = vendor.Name;
            result.SiteUrl = vendor.SiteUrl;
           
            result.Handle = vendor.SeoInfo != null ? vendor.SeoInfo.Slug : vendor.Id;

            var shopifyAddressModels = vendor.Addresses.Select(a => ToLiquidAddress(a));
            result.Addresses = new MutablePagedList<Address>(shopifyAddressModels);
            result.DynamicProperties = vendor.DynamicProperties;

            if (vendor.Products != null)
            {
                result.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos) =>
                {
                    vendor.Products.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Product>(vendor.Products.Select(x => ToLiquidProduct(x)), vendor.Products);
                }, vendor.Products.PageNumber, vendor.Products.PageSize);
            }

            return result;
        }
    }

}