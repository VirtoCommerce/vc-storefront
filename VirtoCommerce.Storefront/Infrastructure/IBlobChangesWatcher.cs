using Microsoft.Extensions.Primitives;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public interface IBlobChangesWatcher
    {
        IChangeToken CreateBlobChangeToken(string path);
    }
}
