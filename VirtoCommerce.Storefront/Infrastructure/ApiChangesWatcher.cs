using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.AutoRestClients.CacheModuleApi;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class ApiChangesWatcher : IApiChangesWatcher
    {
        private readonly ICacheModule _cacheApi;
        private readonly StorefrontOptions _options;

        public ApiChangesWatcher(ICacheModule cacheApi, IOptions<StorefrontOptions> options)
        {
            _cacheApi = cacheApi;
            _options = options.Value;
        }

        public IChangeToken CreateChangeToken()
        {
            return new PollingApiChangeToken(_cacheApi, _options.ChangesPollingInterval);
        }
    }
}
