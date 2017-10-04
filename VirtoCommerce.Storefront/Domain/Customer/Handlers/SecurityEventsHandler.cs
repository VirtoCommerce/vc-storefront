using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Security.Events;

namespace VirtoCommerce.Storefront.Domain.Customer.Handlers
{
    public class SecurityEventsHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly IMemberService _memberService;
        public SecurityEventsHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        #region IEventHandler<UserRegisteredEvent> members
        public virtual async Task Handle(UserRegisteredEvent @event)
        {
            //Need to create new contact related to new user with same Id
            var registrationData = @event.RegistrationInfo;
            var contact = new Contact
            {
                Id = @event.User.Id,
                Name = registrationData.UserName,
                FullName = string.Join(" ", registrationData.FirstName, registrationData.LastName),
                FirstName = registrationData.FirstName,
                LastName = registrationData.LastName
            };
            if (!string.IsNullOrEmpty(registrationData.Email))
            {
                contact.Emails.Add(registrationData.Email);
            }
            if (string.IsNullOrEmpty(contact.FullName) || string.IsNullOrWhiteSpace(contact.FullName))
            {
                contact.FullName = registrationData.Email;
            }
            await _memberService.CreateContactAsync(contact);
        }
        #endregion
    }
}
