using VirtoCommerce.Storefront.Model.Common.Messages;

namespace VirtoCommerce.Storefront.Model.Common.Events
{
    public interface ICancellableEventHandler<in T> : ICancellableHandler<T> where T : IEvent
    {
    }
}