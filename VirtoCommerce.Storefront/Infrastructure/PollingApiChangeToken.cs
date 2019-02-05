using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using VirtoCommerce.Storefront.AutoRestClients.CacheModuleApi;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class PollingApiChangeToken : IChangeToken
    {
        private readonly ICacheModule _cacheApi;
        private static DateTime _previousChangeTimeUtcStatic;
        private static DateTime _lastCheckedTimeUtcStatic;
        private DateTime _previousChangeTimeUtc;
        private readonly TimeSpan _pollingInterval;
        private static object _lock = new object();

        public PollingApiChangeToken(ICacheModule cacheApi, TimeSpan poolingInterval)
        {
            _pollingInterval = poolingInterval;
            _cacheApi = cacheApi;
            _previousChangeTimeUtc = _previousChangeTimeUtcStatic;
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
                var hasChanged = _previousChangeTimeUtc < _previousChangeTimeUtcStatic;

                var currentTime = DateTime.UtcNow;
                if (currentTime - _lastCheckedTimeUtcStatic < _pollingInterval)
                {
                    return hasChanged;
                }

                //Need to prevent API flood for multiple token instances
                bool lockTaken = Monitor.TryEnter(_lock);
                try
                {
                    if (lockTaken)
                    {
                        var lastChangeTimeUtc = GetLastChangeTimeUtc();
                        if (_previousChangeTimeUtcStatic < lastChangeTimeUtc)
                        {
                            _previousChangeTimeUtcStatic = lastChangeTimeUtc;
                            hasChanged = true;
                        }
                        _lastCheckedTimeUtcStatic = currentTime;
                    }
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(_lock);
                }

                return hasChanged;
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
