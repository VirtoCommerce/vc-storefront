using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class TaxLineConverter
    {
        public static TaxLine ToShopifyModel(this Storefront.Model.TaxDetail taxDetail)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidTaxLine(taxDetail);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual TaxLine ToLiquidTaxLine(Storefront.Model.TaxDetail taxDetail)
        {
            var result = new TaxLine();

            result.Price = taxDetail.Amount.Amount * 100;
            result.Title = taxDetail.Name;

            return result;
        }
    }
}