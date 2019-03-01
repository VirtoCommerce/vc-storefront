using Microsoft.Extensions.Caching.Memory;

namespace VirtoCommerce.Storefront.Model.Caching
{
    // This abstraction is used as a substitute for built-in IMemoryCache as a custom way to handle caching logic, impl of it should work as a wrapper
    public interface IStorefrontMemoryCache : IMemoryCache
    {
        MemoryCacheEntryOptions GetDefaultCacheEntryOptions();
    }
}
