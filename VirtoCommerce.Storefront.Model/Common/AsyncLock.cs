using System.Collections.Concurrent;

namespace VirtoCommerce.Storefront.Model.Common
{

    public static class AsyncLock
    {
        private static readonly ConcurrentDictionary<string, Nito.AsyncEx.AsyncLock> _lockMap = new ConcurrentDictionary<string, Nito.AsyncEx.AsyncLock>();

        public static Nito.AsyncEx.AsyncLock GetLockByKey(string key)
        {
            return _lockMap.GetOrAdd(key, (x) => new Nito.AsyncEx.AsyncLock());
        }

    }
}
