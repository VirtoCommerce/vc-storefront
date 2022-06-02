using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PagedList.Core;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class StaticContentInThemeWorkContextBuilderExtensions
    {
        public static Task WithPagesInThemeAsync(this IWorkContextBuilder builder, IMutablePagedList<ContentItem> pages)
        {
            builder.WorkContext.PagesInTheme = pages;
            return Task.CompletedTask;
        }

        public static Task WithTemplatesInThemeAsync(this IWorkContextBuilder builder, IMutablePagedList<ContentItem> templates)
        {
            builder.WorkContext.Templates = templates;
            return Task.CompletedTask;
        }

        public static Task WithPagesInThemeAsync(this IWorkContextBuilder builder, Store store, Language language)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            if (language == null)
            {
                throw new ArgumentNullException(nameof(language));
            }

            var serviceProvider = builder.HttpContext.RequestServices;
            var staticContentService = serviceProvider.GetRequiredService<IStaticContentInThemeService>();

            IPagedList<ContentItem> Factory(int pageNumber, int pageSize, IEnumerable<SortInfo> sorInfos)
            {
                var contentItems = staticContentService.LoadStoreStaticContent(store).Where(x => x.Language.IsInvariant || x.Language == language);
                return new StaticPagedList<ContentItem>(contentItems, pageNumber, pageSize, contentItems.Count());
            }

            return builder.WithPagesInThemeAsync(new MutablePagedList<ContentItem>(Factory, 1, 20));
        }

        public static Task WithTemplatesInThemeAsync(this IWorkContextBuilder builder, Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            var serviceProvider = builder.HttpContext.RequestServices;
            var staticContentService = serviceProvider.GetRequiredService<IStaticContentInThemeService>();

            // all static content items
            IPagedList<ContentItem> Factory(int pageNumber, int pageSize, IEnumerable<SortInfo> sorInfos)
            {
                var contentItems = staticContentService.LoadStoreStaticTemplates(store);
                return new StaticPagedList<ContentItem>(contentItems, pageNumber, pageSize, contentItems.Count());
            }

            return builder.WithTemplatesInThemeAsync(new MutablePagedList<ContentItem>(Factory, 1, 20));
        }
    }
}
