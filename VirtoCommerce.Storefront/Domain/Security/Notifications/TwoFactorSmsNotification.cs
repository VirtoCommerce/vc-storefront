using System.Collections.Generic;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Notifications;

namespace VirtoCommerce.Storefront.Domain.Security.Notifications
{
    public class TwoFactorSmsNotification : SmsNotificationBase
    {
        public TwoFactorSmsNotification(string storeId, Language language)
            : base(storeId, language)
        {
        }

        public string Token { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            foreach (var kvp in base.GetNotificationParameters())
            {
                yield return kvp;
            }
            yield return new KeyValuePair<string, string>(nameof(Token), Token);
        }
    }
}
