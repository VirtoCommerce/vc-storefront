using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class CartConverter
    {
        public static Cart ToShopifyModel(this StorefrontModel.Cart.ShoppingCart cart, StorefrontModel.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidCart(cart, language, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Cart ToLiquidCart(StorefrontModel.Cart.ShoppingCart cart, StorefrontModel.Language language, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new Cart();

            result.Items = cart.Items.Select(x => ToLiquidLineItem(x, language, urlBuilder)).ToList();
            result.ItemCount = cart.Items.Count();
            result.Note = cart.Comment;
            result.TotalPrice = cart.SubTotal.Amount * 100;
            result.TotalWeight = cart.Weight;

            return result;
        }
    }
}