using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class NotificationConverter
    {
        public static Notification ToShopifyModel(this StorefrontNotification notification)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidNotification(notification);
        }
    }
    public partial class ShopifyModelConverter
    {
        public virtual Notification ToLiquidNotification(StorefrontNotification notification)
        {
            var result = new Notification();
            result.Message = notification.Message;
            result.Title = notification.Title;
            result.Type = notification.Type.ToString();

            return result;
        }
    }
}