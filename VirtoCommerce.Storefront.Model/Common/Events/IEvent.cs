using System;
using VirtoCommerce.Storefront.Model.Common.Messages;

namespace VirtoCommerce.Storefront.Model.Common.Events
{
    public interface IEvent : IMessage
    {
        Guid Id { get; set; }
        int Version { get; set; }
        DateTimeOffset TimeStamp { get; set; }
    }
}
