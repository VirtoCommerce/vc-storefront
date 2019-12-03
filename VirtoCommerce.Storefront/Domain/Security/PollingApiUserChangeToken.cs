using System;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public sealed class PollingApiUserChangeToken : IChangeToken
    {
        private readonly ISecurity _platformSecurityApi;
        private static DateTime _previousChangeTimeUtcStatic;
        private static DateTime _lastCheckedTimeUtcStatic;
        private readonly TimeSpan _pollingInterval;

        private readonly object _lock = new object();

        static PollingApiUserChangeToken()
        {
            _previousChangeTimeUtcStatic = _lastCheckedTimeUtcStatic = DateTime.UtcNow;
        }

        public PollingApiUserChangeToken(ISecurity platformSecurityApi, TimeSpan pollingInterval)
        {
            _pollingInterval = pollingInterval;
            _platformSecurityApi = platformSecurityApi;
        }

        /// <summary>
        /// Always false.
        /// </summary>
        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                var currentTime = DateTime.UtcNow;
                if (currentTime - _lastCheckedTimeUtcStatic < _pollingInterval)
                {
                    return false;
                }

                var lockTaken = Monitor.TryEnter(_lock);
                try
                {
                    //Do not wait if is locked by another thread 
                    if (lockTaken)
                    {
                        var result = _platformSecurityApi.SearchUsersAsync(new UserSearchCriteria()
                        {
                            Skip = 0,
                            Take = int.MaxValue,
                            ModifiedSinceDate = _previousChangeTimeUtcStatic
                        });

                        if (result.Result.TotalCount > 0)
                        {
                            _previousChangeTimeUtcStatic = currentTime;
                            foreach (var userId in result.Result.Users.Select(x => x.Id))
                            {
                                SecurityCacheRegion.ExpireUser(userId);
                            }
                        }
                        _lastCheckedTimeUtcStatic = currentTime;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(_lock);
                    }
                }
                return false;
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
