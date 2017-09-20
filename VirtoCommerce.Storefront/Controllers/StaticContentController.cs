using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Controllers
{
    public class StaticContentController : StorefrontControllerBase
    {
        public StaticContentController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
        }

        public ActionResult GetContentPage()
        {
            //Because MVC does not allow to use abstract types for model binding we are getting this value from route data
            var page = RouteData.Values.GetValueOrDefault("page") as ContentItem;
            WorkContext.CurrentPageSeo = new SeoInfo
            {
                Language = page.Language,
                MetaDescription = string.IsNullOrEmpty(page.Description) ? page.Title : page.Description,
                Title = page.Title,
                Slug = page.Url
            };

            var blogArticle = page as BlogArticle;
            if (blogArticle != null)
            {
                WorkContext.CurrentPageSeo.ImageUrl = blogArticle.ImageUrl;
                WorkContext.CurrentPageSeo.MetaDescription = blogArticle.Excerpt ?? blogArticle.Title;

                WorkContext.CurrentBlogArticle = blogArticle;
                WorkContext.CurrentBlog = WorkContext.Blogs.SingleOrDefault(x => x.Name.EqualsInvariant(blogArticle.BlogName));
                var layout = string.IsNullOrEmpty(blogArticle.Layout) ? WorkContext.CurrentBlog.Layout : blogArticle.Layout;
                return View("article", WorkContext);
            }

            var contentPage = page as ContentPage;
            SetCurrentPage(contentPage);
            return View(contentPage.Template, WorkContext);
        }

        // GET: /pages/{page}
        public ActionResult GetContentPageByName(string page)
        {
            var contentPage = WorkContext.Pages
                .OfType<ContentPage>()
                .Where(x => string.Equals(x.Url, page, StringComparison.OrdinalIgnoreCase))
                .FindWithLanguage(WorkContext.CurrentLanguage);

            if (contentPage != null)
            {
                SetCurrentPage(contentPage);
                return View(contentPage.Template, WorkContext);
            }

            return NotFound();
        }

        // GET: /blogs/{blog}, /blog, /blog/category/category, /blogs/{blog}/category/{category}, /blogs/{blog}/tag/{tag}, /blog/tag/{tag}
        public ActionResult GetBlog(string blog = null, string category = null, string tag = null)
        {
            var context = WorkContext;
            context.CurrentBlog = WorkContext.Blogs.FirstOrDefault();
            if (!string.IsNullOrEmpty(blog))
            {
                context.CurrentBlog = WorkContext.Blogs.FirstOrDefault(x => x.Name.EqualsInvariant(blog));
            }
            WorkContext.CurrentBlogSearchCritera.Category = category;
            WorkContext.CurrentBlogSearchCritera.Tag = tag;
            if (context.CurrentBlog != null)
            {
                context.CurrentPageSeo = new SeoInfo
                {
                    Language = context.CurrentBlog.Language,
                    MetaDescription = context.CurrentBlog.Title ?? context.CurrentBlog.Name,
                    Title = context.CurrentBlog.Title ?? context.CurrentBlog.Name,
                    Slug = context.RequestUrl.AbsolutePath
                };
                return View("blog", WorkContext);
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult Search([FromBody] StaticContentSearchCriteria request)
        {
            if (request == null)
            {
                new ArgumentNullException(nameof(request));
            }
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var contentItems = WorkContext.Pages.Where(i =>
                !string.IsNullOrEmpty(i.Content) && i.Content.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                !string.IsNullOrEmpty(i.Title) && i.Title.IndexOf(request.Keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                if (!string.IsNullOrEmpty(request.SearchIn))
                {
                    contentItems = contentItems.Where(i => !string.IsNullOrEmpty(i.StoragePath) && i.StoragePath.StartsWith(request.SearchIn, StringComparison.OrdinalIgnoreCase));
                }

                WorkContext.StaticContentSearchResult = new MutablePagedList<ContentItem>(contentItems.Where(x => x.Language.IsInvariant || x.Language == WorkContext.CurrentLanguage));
            }

            return View("search", WorkContext);
        }

        /// <summary>
        /// GET blogs/{blogname}/rss,  blogs/rss,  blogs/{blogname}/feed,  blogs/feed
        /// </summary>
        /// <param name="blogName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BlogRssFeed(string blogName)
        {
            Blog blog = WorkContext.Blogs.FirstOrDefault();
            if (!string.IsNullOrEmpty(blogName))
            {
                WorkContext.CurrentBlog = WorkContext.Blogs.FirstOrDefault(x => x.Name.EqualsInvariant(blogName));
            }

            if (blog == null)
            {
                return NotFound();
            }

            //TODO: Rss feed
            //var feedItems = new List<SyndicationItem>();
            //foreach(var article in blog.Articles.OrderByDescending(a => a.PublishedDate))
            //{
            //    if (!string.IsNullOrEmpty(article.Url))
            //    {
            //        var baseUri = new Uri(Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host);
            //        var fullUrl = new Uri(baseUri, UrlBuilder.ToAppAbsolute(article.Url, WorkContext.CurrentStore, WorkContext.CurrentLanguage));
            //        var syndicationItem = new SyndicationItem(article.Title, article.Excerpt, fullUrl);
            //        syndicationItem.PublishDate = article.PublishedDate.HasValue ? new DateTimeOffset(article.PublishedDate.Value) : new DateTimeOffset();
            //        feedItems.Add(syndicationItem);
            //    }
            //}

            //var feed = new SyndicationFeed(blog.Title, blog.Title, new Uri(blog.Url, UriKind.Relative), feedItems)
            //{
            //    Language = WorkContext.CurrentLanguage.CultureName,
            //    Title = new TextSyndicationContent(blog.Title)
            //};

            //return new FeedResult(new Rss20FeedFormatter(feed));
            throw new NotImplementedException();
        }

        private void SetCurrentPage(ContentPage contentPage)
        {
            WorkContext.CurrentPage = contentPage;
            WorkContext.CurrentPageSeo = new SeoInfo
            {
                Language = contentPage.Language,
                MetaDescription = string.IsNullOrEmpty(contentPage.Description) ? contentPage.Title : contentPage.Description,
                Title = contentPage.Title,
                Slug = contentPage.Permalink
            };
        }
    }
}