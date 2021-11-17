using System;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class PollingApiChangeToken : IChangeToken
    {
        private readonly IChangeLog _cacheApi;
        private static DateTime _previousChangeTimeUtcStatic;
        private static DateTime _lastCheckedTimeUtcStatic;
        private readonly DateTime _previousChangeTimeUtc;
        private readonly TimeSpan _pollingInterval;
        private static readonly object _lock = new object();

        public PollingApiChangeToken(IChangeLog cacheApi, TimeSpan pollingInterval)
        {
            _pollingInterval = pollingInterval;
            _cacheApi = cacheApi;
            _previousChangeTimeUtc = _previousChangeTimeUtcStatic;
        }

        public static void UpdatePreviousChangeTimeUtcStatic(DateTime currentTime)
        {
            _previousChangeTimeUtcStatic = currentTime;
        }

        public static void UpdateLastCheckedTimeUtcStatic(DateTime currentTime)
        {
            _lastCheckedTimeUtcStatic = currentTime;
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
                var lockTaken = Monitor.TryEnter(_lock);

                try
                {
                    if (lockTaken)
                    {
                        var lastChangeTimeUtc = GetLastChangeTimeUtc();
                        if (_previousChangeTimeUtcStatic < lastChangeTimeUtc)
                        {
                            UpdatePreviousChangeTimeUtcStatic(lastChangeTimeUtc);
                            hasChanged = true;
                        }

                        UpdateLastCheckedTimeUtcStatic(currentTime);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(_lock);
                    }
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
