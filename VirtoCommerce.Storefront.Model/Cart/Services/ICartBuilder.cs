using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.BulkOrder;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Cart.Services
{
    /// <summary>
    /// Represent abstraction for working with customer shopping cart
    /// </summary>
    public interface ICartBuilder
    {
        ShoppingCart Cart { get; }

        /// <summary>
        ///  Capture cart and all next changes will be implemented on it
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        Task TakeCartAsync(ShoppingCart cart);

        /// <summary>
        /// Update shopping cart comment
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task UpdateCartComment(string comment);

        /// <summary>
        /// Update purchase order number
        /// </summary>
        /// <param name="purchaseOrderNumber"></param>
        /// <returns></returns>
        Task UpdatePurchaseOrderNumberAsync(string purchaseOrderNumber);

        /// <summary>
        /// Load or created new cart for specified parameters and capture it.  All next changes will be implemented on it
        /// </summary>
        /// <param name="cartName"></param>
        /// <param name="store"></param>
        /// <param name="user"></param>
        /// <param name="language"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        Task LoadOrCreateNewTransientCartAsync(string cartName, Store store, User user, Language language, Currency currency, string type = null);

        void LoadOrCreateNewTransientCart(string cartName, Store store, User user, Language language, Currency currency, string type = null);

        /// <summary>
        /// Add new product to cart
        /// </summary>
        /// <param name="product"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<bool> AddItemAsync(AddCartItem addCartItem);

        /// <summary>
        /// Change cart item qty by product index
        /// </summary>
        Task ChangeItemQuantityAsync(ChangeCartItemQty changeItemQty);

        /// <summary>
        /// Change cart item qty by item id
        /// </summary>
        /// <param name="lineItemIndex"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task ChangeItemQuantityAsync(int lineItemIndex, int quantity);

        Task ChangeItemsQuantitiesAsync(int[] quantities);

        Task ChangeItemPriceAsync(ChangeCartItemPrice newPrice);

        Task ChangeItemCommentAsync(ChangeCartItemComment newItemComment);

        Task ChangeItemDynamicPropertiesAsync(ChangeCartItemDynamicProperties newItemDynamicProperties);

        /// <summary>
        /// Remove item from cart by id
        /// </summary>
        /// <param name="lineItemId"></param>
        /// <returns></returns>
        Task RemoveItemAsync(string lineItemId);

        /// <summary>
        /// Apply marketing coupon to captured cart
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        Task AddCouponAsync(string couponCode);

        /// <summary>
        /// remove exist coupon from cart
        /// </summary>
        /// <param name="couponCode"></param>
        /// <returns></returns>
        Task RemoveCouponAsync(string couponCode = null);

        /// <summary>
        /// Clear cart remove all items and shipments and payments
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        /// Add or update shipment to cart
        /// </summary>
        /// <param name="shipment"></param>
        /// <returns></returns>
        Task AddOrUpdateShipmentAsync(Shipment shipment);

        /// <summary>
        /// Remove exist shipment from cart
        /// </summary>
        /// <param name="shipmentId"></param>
        /// <returns></returns>
        Task RemoveShipmentAsync(string shipmentId);

        /// <summary>
        /// Add or update payment in cart
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        Task AddOrUpdatePaymentAsync(Payment payment);

        /// <summary>
        /// Merge other cart with captured
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        Task MergeWithCartAsync(ShoppingCart cart);

        /// <summary>
        /// Remove cart from service
        /// </summary>
        /// <returns></returns>
        Task RemoveCartAsync();

        /// <summary>
        /// Fill current captured cart from RFQ
        /// </summary>
        /// <param name="quoteRequest"></param>
        /// <returns></returns>
        Task FillFromQuoteRequestAsync(QuoteRequest quoteRequest);

        /// <summary>
        /// Returns all available shipment methods for current cart
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync();

        /// <summary>
        /// Returns all available payment methods for current cart
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync();

        /// <summary>
        /// Evaluate marketing discounts for captured cart
        /// </summary>
        /// <returns></returns>
        Task EvaluatePromotionsAsync();

        /// <summary>
        /// Evaluate taxes  for captured cart
        /// </summary>
        /// <returns></returns>
        Task EvaluateTaxesAsync();

        Task ValidateAsync();

        Task SaveAsync();
    }
}
