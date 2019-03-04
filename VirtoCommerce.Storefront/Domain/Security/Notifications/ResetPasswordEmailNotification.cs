using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Notifications;

namespace VirtoCommerce.Storefront.Domain.Security.Notifications
{
    public class ResetPasswordEmailNotification : EmailNotificationBase
    {
        public ResetPasswordEmailNotification(string storeId, Language language)
            : base(storeId, language)
        {
        }

        public string Url { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            var result = base.GetNotificationParameters().ToList();
            result.Add(new KeyValuePair<string, string>(nameof(Url), Url));
            return result;
        }
    }
}
