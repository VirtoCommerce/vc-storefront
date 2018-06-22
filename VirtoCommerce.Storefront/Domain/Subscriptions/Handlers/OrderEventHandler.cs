using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Order.Events;

namespace VirtoCommerce.Storefront.Domain.Handlers
{
    public class OrderEventsHandler : IEventHandler<OrderPlacedEvent>
    {
        public virtual Task Handle(OrderPlacedEvent @event)
        {
            if (@event.Order != null)
            {
                SubscriptionCacheRegion.ExpireCustomerSubscription(@event.Order.CustomerId);
            }
            return Task.CompletedTask;
        }
    }
}
