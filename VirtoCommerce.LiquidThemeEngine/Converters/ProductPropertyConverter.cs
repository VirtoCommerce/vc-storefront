using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ProductPropertyConverter
    {
        public static ProductProperty ToShopifyModel(this StorefrontModel.Catalog.CatalogProperty property)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidProductProperty(property);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual ProductProperty ToLiquidProductProperty(StorefrontModel.Catalog.CatalogProperty property)
        {
            var result = new ProductProperty();
            result.ValueType = property.ValueType;
            result.Value = property.Value;
            result.Name = property.Name;
            result.DisplayName = property.DisplayName ?? property.Name;
            return result;
        }
    }

}