using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class ApiChangesWatcher : IApiChangesWatcher
    {
        private readonly IChangeLog _cacheApi;
        private readonly StorefrontOptions _options;

        public ApiChangesWatcher(IChangeLog cacheApi, IOptions<StorefrontOptions> options)
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
