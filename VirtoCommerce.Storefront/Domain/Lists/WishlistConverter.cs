using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Lists;
using VirtoCommerce.Storefront.Model.Security;
using cartDto = VirtoCommerce.Storefront.AutoRestClients.CartModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain.Lists
{
    public static class WishlistConverterConverterExtension
    {
        public static WishlistConverter WishlistConverterInstance
        {
            get
            {
                return new WishlistConverter();
            }
        }

        public static Wishlist ToWishlist(this cartDto.ShoppingCart cartDto, Currency currency, Language language, User user)
        {
            return WishlistConverterInstance.ToWishlist(cartDto, currency, language, user);
        }

        public static Wishlist ToWishlist(this ShoppingCart cart, Currency currency, Language language, User user)
        {
            return WishlistConverterInstance.ToWishlist(cart, currency, language, user);
        }

        public static cartDto.ShoppingCartSearchCriteria ToSearchCriteriaDto(this WishlistSearchCriteria criteria)
        {
            return WishlistConverterInstance.ToSearchCriteriaDto(criteria);
        }
    }

    public partial class WishlistConverter : CartConverter
    {
        public virtual Wishlist ToWishlist(cartDto.ShoppingCart cartDto, Currency currency, Language language, User user)
        {
            var cart = base.ToShoppingCart(cartDto, currency, language, user);
            var result = cart.ToWishlist(currency, language, user);
            return result;
        }

        public virtual Wishlist ToWishlist(ShoppingCart cart, Currency currency, Language language, User user)
        {
            var result = new Wishlist(currency, language);

            result.ChannelId = cart.ChannelId;
            result.Comment = cart.Comment;
            result.CustomerId = cart.CustomerId;
            result.CustomerName = cart.CustomerName;
            result.Id = cart.Id;
            result.Name = cart.Name;
            result.ObjectType = cart.ObjectType;
            result.OrganizationId = cart.OrganizationId;
            result.Status = cart.Status;
            result.StoreId = cart.StoreId;
            result.Type = cart.Type;
            result.Customer = cart.Customer;
            result.Coupon = cart.Coupon;
            result.Items = cart.Items;
            result.HasPhysicalProducts = cart.HasPhysicalProducts;
            result.Addresses = cart.Addresses;
            result.Payments = cart.Payments;
            result.Shipments = cart.Shipments;
            result.DynamicProperties = cart.DynamicProperties;
            result.TaxDetails = cart.TaxDetails;
            result.DiscountAmount = cart.DiscountAmount;
            result.HandlingTotal = cart.HandlingTotal;
            result.HandlingTotalWithTax = cart.HandlingTotalWithTax;
            result.IsAnonymous = cart.IsAnonymous;
            result.IsRecuring = cart.IsRecuring;
            result.VolumetricWeight = cart.VolumetricWeight;
            result.Weight = cart.Weight;

            return result;
        }

        public virtual cartDto.ShoppingCartSearchCriteria ToSearchCriteriaDto(WishlistSearchCriteria criteria)
        {
            var result = new cartDto.ShoppingCartSearchCriteria();

            result.Name = criteria.Name;
            result.Type = criteria.Type;
            result.StoreId = criteria.StoreId;
            result.CustomerId = criteria.Customer?.Id;
            result.Currency = criteria.Currency?.Code;
            result.LanguageCode = criteria.Language?.CultureName;

            result.Skip = criteria.Start;
            result.Take = criteria.PageSize;
            result.Sort = criteria.Sort;

            return result;
        }
    }
}
