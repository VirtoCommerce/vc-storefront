using Microsoft.Extensions.Primitives;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public interface IApiChangesWatcher
    {
        IChangeToken CreateChangeToken();
    }
}
