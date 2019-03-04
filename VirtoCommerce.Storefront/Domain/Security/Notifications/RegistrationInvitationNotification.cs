using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Notifications;

namespace VirtoCommerce.Storefront.Domain.Security.Notifications
{
    public class RegistrationInvitationNotification : EmailNotificationBase
    {
        public RegistrationInvitationNotification(string storeId, Language language)
            : base(storeId, language)
        {
        }

        public string InviteUrl { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            var result = base.GetNotificationParameters().ToList();
            result.Add(new KeyValuePair<string, string>(nameof(InviteUrl), InviteUrl));
            return result;
        }
    }
}
