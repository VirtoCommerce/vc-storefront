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
        public static Task WithMenuLinksAsync(this IWorkContextBuilder builder, IMutablePagedList<MenuLinkList> linkLists)
        {
            builder.WorkContext.CurrentLinkLists = linkLists;
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

            IPagedList<MenuLinkList> Factory(int pageNumber, int pageSize, IEnumerable<SortInfo> sorInfos)
            {
                var linkLists = linkListService.LoadAllStoreLinkLists(store, language);
                return new StaticPagedList<MenuLinkList>(linkLists, pageNumber, pageSize, linkLists.Count());
            }

            return builder.WithMenuLinksAsync(new MutablePagedList<MenuLinkList>((Func<int, int, IEnumerable<SortInfo>, IPagedList<MenuLinkList>>) Factory, 1, 20));
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
            IPagedList<ContentItem> Factory(int pageNumber, int pageSize, IEnumerable<SortInfo> sorInfos)
            {
                var contentItems = staticContentService.LoadStoreStaticContent(store).Where(x => x.Language.IsInvariant || x.Language == language);
                return new StaticPagedList<ContentItem>(contentItems, pageNumber, pageSize, contentItems.Count());
            }

            return builder.WithPagesAsync(new MutablePagedList<ContentItem>((Func<int, int, IEnumerable<SortInfo>, IPagedList<ContentItem>>) Factory, 1, 20));
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

            IPagedList<Blog> Factory(int pageNumber, int pageSize, IEnumerable<SortInfo> sorInfos)
            {
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
            }

            // Initialize blogs search criteria 
            builder.WorkContext.CurrentBlogSearchCritera = new BlogSearchCriteria(builder.WorkContext.QueryString);

            return builder.WithBlogsAsync(new MutablePagedList<Blog>((Func<int, int, IEnumerable<SortInfo>, IPagedList<Blog>>) Factory, 1, 20));
        }
    }
}
