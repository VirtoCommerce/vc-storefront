using Microsoft.Extensions.Primitives;
using System.Threading;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    public class CancellableCacheRegion<T>
    {
        private static CancellationTokenSource _regionTokenSource;
        private static CancellationChangeToken _regionChangeToken;
        private static object _lock = new object();
       
        public static IChangeToken GetChangeToken()
        {
            if(_regionTokenSource == null)
            {
                lock (_lock)
                {
                    if (_regionTokenSource == null)
                    {
                        _regionTokenSource = new CancellationTokenSource();
                        _regionChangeToken = new CancellationChangeToken(_regionTokenSource.Token);
                    }
                }
            }
            return _regionChangeToken;
        }


        public static void ClearRegion()
        {
            lock (_lock)
            {
                _regionTokenSource.Cancel();
                _regionTokenSource.Dispose();
                //Need to reset cached tokens because they are already changed
                _regionTokenSource = null;
                _regionChangeToken = null;
            }

        }

    }
}
