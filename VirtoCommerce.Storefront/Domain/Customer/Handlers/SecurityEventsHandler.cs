using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Security.Events;

namespace VirtoCommerce.Storefront.Domain.Customer.Handlers
{
    public class SecurityEventsHandler : IEventHandler<UserDeletedEvent>
    {
        private readonly IMemberService _memberService;
        public SecurityEventsHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        #region IEventHandler<UserRegisteredEvent> members

        public async Task Handle(UserDeletedEvent message)
        {
            if (message.User.ContactId != null)
            {
                await _memberService.DeleteContactAsync(message.User.ContactId);
            }
        }
        #endregion
    }
}
