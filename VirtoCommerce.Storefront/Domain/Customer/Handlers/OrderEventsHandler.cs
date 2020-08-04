using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
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

        public virtual async Task Handle(OrderPlacedEvent @event)
        {
            if (@event.Order != null)
            {
                //Add addresses to contact profile
                var contact = @event.WorkContext.CurrentUser?.Contact;
                if (contact != null)
                {
                    var addresses = contact.Addresses
                        .Concat(@event.Order.Addresses.Select(x => new Address().CopyFrom(x, @event.WorkContext.Countries?.ToArray() ?? Array.Empty<Country>())))
                        .Concat(@event.Order.Shipments.Select(shipment => new Address().CopyFrom(shipment.DeliveryAddress, @event.WorkContext.Countries?.ToArray() ?? Array.Empty<Country>())))
                        .Where(address => address != null)
                        .Distinct()
                        .ToList();

                    if (!addresses.SequenceEqual(contact.Addresses))
                    {
                        foreach (var address in addresses)
                        {
                            address.Name = address.ToString();
                            address.Name = address.Name.Substring(0, Math.Min(1800, address.Name.Length));
                        }
                        await _memberService.UpdateContactAddressesAsync(contact.Id, addresses);
                    }
                }
            }
        }
    }
}
