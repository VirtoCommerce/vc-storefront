using VirtoCommerce.Storefront.Model.Common.Events;

namespace VirtoCommerce.Storefront.Model.Security.Events
{
    public class UserDeletedEvent : DomainEvent
    {
        public UserDeletedEvent(WorkContext workContext, User user)
        {
            WorkContext = workContext;
            User = user;
        }

        public WorkContext WorkContext { get; set; }
        public User User { get; set; }
    }
}
