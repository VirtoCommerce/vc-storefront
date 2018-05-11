using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public abstract class EmailNotificationBase : ValueObject
    {
        public EmailNotificationBase(string storeId, Language language)
        {
            Type = GetType().Name;
            StoreId = storeId;
            Language = language;
        }

        public string Type { get; private set; }
        public string StoreId { get; set; }
        public Language Language { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }

        public virtual IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            yield return new KeyValuePair<string, string>(nameof(Sender), Sender);
            yield return new KeyValuePair<string, string>(nameof(Recipient), Recipient);
        }
    }
}
