namespace VirtoCommerce.Storefront.Model.Common.Notifications
{
    public abstract class SmsNotificationBase : NotificationBase
    {
        protected SmsNotificationBase(string storeId, Language language)
            : base(storeId, language)
        {
        }
    }
}
