using Microsoft.Extensions.Primitives;
using System.Threading;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    public class CancellableCacheRegion<T>
    {
        private static readonly CancellationTokenSource _regionTokenSource;
        private static readonly CancellationChangeToken _regionChangeToken;     

        static CancellableCacheRegion()
        {
            _regionTokenSource = new CancellationTokenSource();
            _regionChangeToken = new CancellationChangeToken(_regionTokenSource.Token);
        }

        public static IChangeToken GetChangeToken()
        {
            return _regionChangeToken;
        }


        public static void ClearRegion()
        {
            _regionTokenSource.Cancel();
        }

    }
}
