using VirtoCommerce.Storefront.Model.Common.Events;

namespace VirtoCommerce.Storefront.Model.Security.Events
{
    public class UserRegisteredEvent : DomainEvent
    {
        public UserRegisteredEvent(WorkContext workContext, User user, UserRegistrationInfo registrationInfo)
        {
            WorkContext = workContext;
            User = user;
            RegistrationInfo = registrationInfo;
        }

        public WorkContext WorkContext { get; set; }
        public User User { get; set; }
        public UserRegistrationInfo  RegistrationInfo { get; set; }
    }
}
