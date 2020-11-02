using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    /// <summary>
    /// Stores all cancellation tokens that are associated for cached entries
    /// </summary>
    public static class CacheCancellableTokensRegistry
    {
        private static ConcurrentDictionary<string, CancellationTokenSource> _tokensDict = new ConcurrentDictionary<string, CancellationTokenSource>();
        // Events are not used intentionally to restrict usage by multiple subscribers
        public static Action<TokenCancelledEventArgs> OnTokenCancelled { get; set; }

        public static IChangeToken CreateChangeToken(string tokenKey)
        {
            var tokenSource = _tokensDict.GetOrAdd(tokenKey, _ => new CancellationTokenSource());
            return new CancellationChangeToken(tokenSource.Token);
        }

        public static bool TryCancelToken(string tokenKey, bool raiseEvent = true)
        {
            var result = _tokensDict.TryRemove(tokenKey, out var token);
            if (result)
            {
                token.Cancel();
                token.Dispose();
            }
            if (raiseEvent)
            {
                //Notify even if no token found for the key.
                //It is important for cached data consistency when scale-out configuration is used, because other instances may contain cached entries with this key
                TriggerOnCancel(tokenKey);
            }
            return result;
        }

        private static void TriggerOnCancel(string tokenKey)
        {
            OnTokenCancelled?.Invoke(new TokenCancelledEventArgs(tokenKey));
        }
    }
}
