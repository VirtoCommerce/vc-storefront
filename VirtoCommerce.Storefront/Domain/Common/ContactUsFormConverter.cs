using System.Linq;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi.Models;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain
{
    public static class ContactUsFormConverter
    {
        public static SendDynamicNotificationRequest ToServiceModel(this ContactForm contactUsForm, WorkContext workContext)
        {
            var retVal = new SendDynamicNotificationRequest
            {
                Language = workContext.CurrentLanguage.CultureName,
                StoreId = workContext.CurrentStore.Id,
                Type = contactUsForm.FormType,
                Fields = contactUsForm.Contact.ToDictionary(x => x.Key, x => x.Value != null ? string.Join(", ", x.Value) : string.Empty)
            };
            return retVal;
        }
    }
}
