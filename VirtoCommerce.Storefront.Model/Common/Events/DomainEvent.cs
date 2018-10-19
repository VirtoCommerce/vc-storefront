using System;

namespace VirtoCommerce.Storefront.Model.Common.Events
{
    public class DomainEvent : IEvent
    {
        public DomainEvent()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
