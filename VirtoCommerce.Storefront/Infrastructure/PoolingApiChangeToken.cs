using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using VirtoCommerce.Storefront.AutoRestClients.CacheModuleApi;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class PoolingApiChangeToken : IChangeToken
    {
        private readonly ICacheModule _cacheApi;
        private static DateTime _previousChangeTimeUtc;
        private static DateTime _lastCheckedTimeUtc;
        private bool _hasChanged;
        private readonly TimeSpan _poolingInterval;
        private static object _lock = new object();

        public PoolingApiChangeToken(ICacheModule cacheApi, TimeSpan poolingInterval)
        {
            _poolingInterval = poolingInterval;
            _cacheApi = cacheApi;
        }

        private DateTime GetLastChangeTimeUtc()
        {
            return _cacheApi.GetLastModifiedDate().LastModifiedDate ?? default(DateTime);
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool ActiveChangeCallbacks => false;
       
        public bool HasChanged
        {
            get
            {
                if (_hasChanged)
                {
                    return _hasChanged;
                }

                var currentTime = DateTime.UtcNow;
                if (currentTime - _lastCheckedTimeUtc < _poolingInterval)
                {
                    return _hasChanged;
                }

                //Need to prevent API flood for multiple token instances
                bool lockTaken = Monitor.TryEnter(_lock);
                try
                {
                    if (lockTaken)
                    {
                        var lastChangeTimeUtc = GetLastChangeTimeUtc();
                        if (_previousChangeTimeUtc < lastChangeTimeUtc)
                        {
                            _previousChangeTimeUtc = lastChangeTimeUtc;
                            _hasChanged = true;
                        }
                        _lastCheckedTimeUtc = currentTime;
                    }                 
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_lock);
                }
             
                return _hasChanged;
            }
        }

        /// <summary>
        /// Does not actually register callbacks.
        /// </summary>
        /// <param name="callback">This parameter is ignored</param>
        /// <param name="state">This parameter is ignored</param>
        /// <returns>A disposable object that noops when disposed</returns>
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => EmptyDisposable.Instance;
    }
}
