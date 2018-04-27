using System.Linq;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain.Common
{
    public static class EmailNotificationConverter
    {
        public static TestNotificationRequest ToNotificationDto(this EmailNotificationBase notification)
        {
            var result = new TestNotificationRequest
            {
                Language = notification.Language.CultureName,
                ObjectId = notification.StoreId,
                ObjectTypeId = "Store",
                Type = notification.Type,
                NotificationParameters = notification.GetNotificationParameters().Select(x => new NotificationParameter { Type = "String", Value = x.Value, ParameterName = x.Key }).ToList()
            };
            return result;
        }
    }
}
