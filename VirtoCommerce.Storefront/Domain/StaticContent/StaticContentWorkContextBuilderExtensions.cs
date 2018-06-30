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
    public static class StaticContentWorkContextBuilderExtensions
    {
        public static Task WithMenuLinksAsync(this IWorkContextBuilder builder, IMutablePagedList<MenuLinkList> linlitsts)
        {
            builder.WorkContext.CurrentLinkLists = linlitsts;
            return Task.CompletedTask;
        }

        public static Task WithMenuLinksAsync(this IWorkContextBuilder builder, Store store, Language language)
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
            var linkListService = serviceProvider.GetRequiredService<IMenuLinkListService>();

            Func<int, int, IEnumerable<SortInfo>, IPagedList<MenuLinkList>> factory = (pageNumber, pageSize, sorInfos) =>
            {
                var linkLists = linkListService.LoadAllStoreLinkLists(store, language);
                return new StaticPagedList<MenuLinkList>(linkLists, pageNumber, pageSize, linkLists.Count());
            };

            return builder.WithMenuLinksAsync(new MutablePagedList<MenuLinkList>(factory, 1, 20));
        }

        public static Task WithPagesAsync(this IWorkContextBuilder builder, IMutablePagedList<ContentItem> pages)
        {
            builder.WorkContext.Pages = pages;
            return Task.CompletedTask;
        }

        public static Task WithPagesAsync(this IWorkContextBuilder builder, Store store, Language language)
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
            var staticContentService = serviceProvider.GetRequiredService<IStaticContentService>();

            // all static content items
            Func<int, int, IEnumerable<SortInfo>, IPagedList<ContentItem>> factory = (pageNumber, pageSize, sorInfos) =>
            {
                //TODO: performance and DDD specification instead if statements
                var contentItems = staticContentService.LoadStoreStaticContent(store).Where(x => x.Language.IsInvariant || x.Language == language);
                return new StaticPagedList<ContentItem>(contentItems, pageNumber, pageSize, contentItems.Count());
            };
            return builder.WithPagesAsync(new MutablePagedList<ContentItem>(factory, 1, 20));
        }

        public static Task WithBlogsAsync(this IWorkContextBuilder builder, IMutablePagedList<Blog> blogs)
        {
            builder.WorkContext.Blogs = blogs;
            return Task.CompletedTask;
        }

        public static Task WithBlogsAsync(this IWorkContextBuilder builder, Store store, Language language)
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
            var staticContentService = serviceProvider.GetRequiredService<IStaticContentService>();

            Func<int, int, IEnumerable<SortInfo>, IPagedList<Blog>> factory = (pageNumber, pageSize, sorInfos) =>
            {
                //TODO: performance and DDD specification instead if statements
                var contentItems = staticContentService.LoadStoreStaticContent(store).Where(x => x.Language.IsInvariant || x.Language == language);
                var blogs = contentItems.OfType<Blog>().ToArray();
                var blogArticlesGroup = contentItems.OfType<BlogArticle>().GroupBy(x => x.BlogName, x => x).ToList();
                foreach (var blog in blogs)
                {
                    var blogArticles = blogArticlesGroup.FirstOrDefault(x => string.Equals(x.Key, blog.Name, StringComparison.OrdinalIgnoreCase));
                    if (blogArticles != null)
                    {
                        blog.Articles = new MutablePagedList<BlogArticle>(blogArticles);
                    }
                }
                return new StaticPagedList<Blog>(blogs, pageNumber, pageSize, blogs.Count());
            };

            // Initialize blogs search criteria 
            builder.WorkContext.CurrentBlogSearchCritera = new BlogSearchCriteria(builder.WorkContext.QueryString);

            return builder.WithBlogsAsync(new MutablePagedList<Blog>(factory, 1, 20));
        }
    }
}
