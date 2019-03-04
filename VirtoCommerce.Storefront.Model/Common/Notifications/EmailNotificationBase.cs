using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common.Notifications
{
    public abstract class EmailNotificationBase : NotificationBase
    {
        protected EmailNotificationBase(string storeId, Language language)
            : base(storeId, language)
        {
        }

        public string Sender { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            foreach (var kvp in base.GetNotificationParameters())
            {
                yield return kvp;
            }

            yield return new KeyValuePair<string, string>(nameof(Sender), Sender);
        }
    }
}
