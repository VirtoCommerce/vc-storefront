using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Lists
{
    public class Wishlist : ShoppingCart
    {
        public Wishlist(Currency currency, Language language) : base(currency, language)
        {
        }
    }
}
