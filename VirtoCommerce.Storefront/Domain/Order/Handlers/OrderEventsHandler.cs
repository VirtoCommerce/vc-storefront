using System.Threading.Tasks;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Order.Events;

namespace VirtoCommerce.Storefront.Domain.Order.Handlers
{
    public class OrderEventsHandler : IEventHandler<OrderPlacedEvent>
    {
        public virtual Task Handle(OrderPlacedEvent message)
        {
            // Need to expire cache for resetting 'IsFirstTimeBuyer' after order creation
            if (message.WorkContext.CurrentUser.IsFirstTimeBuyer)
            {
                SecurityCacheRegion.ExpireUser(message.WorkContext.CurrentUser.Id);
            }

            return Task.CompletedTask;
        }
    }
}
