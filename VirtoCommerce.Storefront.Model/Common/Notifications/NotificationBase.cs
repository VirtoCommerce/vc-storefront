using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common.Notifications
{
    public abstract class NotificationBase : ValueObject
    {
        protected NotificationBase(string storeId, Language language)
        {
            Type = GetType().Name;
            StoreId = storeId;
            Language = language;
        }

        public string Type { get; private set; }
        public string StoreId { get; set; }
        public Language Language { get; set; }
        public string Recipient { get; set; }

        public virtual IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            yield return new KeyValuePair<string, string>(nameof(Recipient), Recipient);
        }
    }
}
