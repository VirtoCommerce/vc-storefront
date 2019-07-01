using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Notifications;

namespace VirtoCommerce.Storefront.Domain.Security.Notifications
{
    public class RegistrationEmailNotification : EmailNotificationBase
    {
        public RegistrationEmailNotification(string storeId, Language language)
            : base(storeId, language)
        {
        }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override IEnumerable<KeyValuePair<string, string>> GetNotificationParameters()
        {
            var result = base.GetNotificationParameters().ToList();
            result.Add(new KeyValuePair<string, string>(nameof(Login), Login));
            result.Add(new KeyValuePair<string, string>(nameof(FirstName), FirstName));
            result.Add(new KeyValuePair<string, string>(nameof(LastName), LastName));
            return result;
        }
    }
}
