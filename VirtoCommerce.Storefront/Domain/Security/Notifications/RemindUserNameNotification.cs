using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain.Security.Notifications
{
    public class RemindUserNameNotification : EmailNotificationBase
    {
        public RemindUserNameNotification(string storeId, Language language)
            : base(storeId, language)
        {
        }

        public string UserName { get; set; }


        public override IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            var result = base.GetNotificationParameters().ToList();
            result.Add(new KeyValuePair<string, string>(nameof(UserName), UserName));
            return result;
        }
    }
}
