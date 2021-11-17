using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.AutoRestClients.ContentModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public class MenuLinkListServiceImpl : IMenuLinkListService
    {
        private readonly IMenu _cmsApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;
        public MenuLinkListServiceImpl(IMenu cmsApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _cmsApi = cmsApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }
        public IList<MenuLinkList> LoadAllStoreLinkLists(Store store, Language language)
        {
            return LoadAllStoreLinkListsAsync(store, language).GetAwaiter().GetResult();
        }

        public Task<IList<MenuLinkList>> LoadAllStoreLinkListsAsync(Store store, Language language)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            return LoadAllStoreLinkListsInternalAsync(store, language);
        }

        public async Task<IList<MenuLinkList>> LoadAllStoreLinkListsInternalAsync(Store store, Language language)
        {
            var cacheKey = CacheKey.With(GetType(), "LoadAllStoreLinkLists", store.Id, language.CultureName);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(StaticContentCacheRegion.CreateChangeToken());
                cacheEntry.AddExpirationToken(_apiChangesWatcher.CreateChangeToken());

                var result = new List<MenuLinkList>();
                var listsDto = await _cmsApi.GetListsAsync(store.Id);
                if (listsDto != null)
                {
                    result.AddRange(listsDto.Select(x => x.ToMenuLinkList()));
                }

                result = result.GroupBy(x => x.Name).Select(x => x.FindWithLanguage(language)).Where(x => x != null).ToList();


                return result.ToList();
            });
        }
    }
}
