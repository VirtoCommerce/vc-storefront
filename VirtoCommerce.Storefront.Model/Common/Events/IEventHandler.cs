
using VirtoCommerce.Storefront.Model.Common.Messages;

namespace VirtoCommerce.Storefront.Model.Common.Events
{
    public interface IEventHandler<in T> : IHandler<T> where T : IEvent
    {
    }
}
