using System.Linq;
using VirtoCommerce.Storefront.AutoRestClients.NotificationsModuleApi.Models;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model.Common.Notifications;

namespace VirtoCommerce.Storefront.Domain.Common
{
    public static class NotificationConverter
    {
        public static NotificationRequest ToNotificationDto(this NotificationBase notification)
        {
            var result = new NotificationRequest
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
