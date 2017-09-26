using VirtoCommerce.Storefront.Model.Common.Events;

namespace VirtoCommerce.Storefront.Model.Customer.Events
{
    public class UserRegisteredEvent : DomainEvent
    {
        public UserRegisteredEvent(WorkContext workContext, CustomerInfo user)
        {
            WorkContext = workContext;
            User = user;
        }

        public WorkContext WorkContext { get; set; }
        public CustomerInfo User { get; set; }
    }
}
