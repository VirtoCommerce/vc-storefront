using System;
using System.Threading;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public sealed class PollingApiUserChangeToken : IChangeToken
    {
        private readonly ISecurity _platformSecurityApi;
        private DateTime _lastCheckedTimeUtc;
        private readonly TimeSpan _poolingInterval;
        private readonly User _user;
        private readonly object _lock = new object();

        private PollingApiUserChangeToken(User user, ISecurity platformSecurityApi, TimeSpan poolingInterval)
        {
            _user = user;
            _lastCheckedTimeUtc = DateTime.UtcNow;
            _poolingInterval = poolingInterval;
            _platformSecurityApi = platformSecurityApi;
        }

        public static IChangeToken CreateChangeToken(User user, ISecurity platformSecurityApi, TimeSpan poolingInterval)
        {
            return new PollingApiUserChangeToken(user, platformSecurityApi, poolingInterval);
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
                if (currentTime - _lastCheckedTimeUtc < _poolingInterval)
                {
                    return false;
                }

                var lockTaken = Monitor.TryEnter(_lock);
                try
                {
                    //Do not wait if is locked by another thread 
                    if (lockTaken)
                    {
                        var user = _platformSecurityApi.GetUserById(_user.Id)?.ToUser();
                        _lastCheckedTimeUtc = currentTime;
                        //TODO: Add additional properties check
                        if (user != null && user.IsLockedOut != _user.IsLockedOut)
                        {
                            SecurityCacheRegion.ExpireUser(_user.Id);
                            return true;
                        }

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
