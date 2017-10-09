using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer.Services;
using VirtoCommerce.Storefront.Model.Order.Events;

namespace VirtoCommerce.Storefront.Domain.Customer.Handlers
{
    public class OrderEventsHandler : IEventHandler<OrderPlacedEvent>
    {
        private readonly IMemberService _memberService;
        public OrderEventsHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        #region IEventHandler<OrderPlacedEvent> members
        public virtual async Task Handle(OrderPlacedEvent @event)
        {
            if (@event.Order != null)
            {
                var contact = @event.WorkContext.CurrentUser?.Contact?.Value;
                //Add addresses to contact profile
                if (contact != null)
                {
                    var addresses = contact.Addresses.Concat(@event.Order.Addresses)
                                                         .Concat(@event.Order.Shipments.Select(x => x.DeliveryAddress))
                                                         .Distinct()
                                                         .ToList();

                    foreach (var address in addresses)
                    {

                        address.Name = address.ToString();
                        address.Name = address.Name.Substring(0, Math.Min(1800, address.Name.Length));
                    }
                    await _memberService.UpdateContactAddressesAsync(contact.Id, addresses);
                }
            }
        }
        #endregion
    }
}

