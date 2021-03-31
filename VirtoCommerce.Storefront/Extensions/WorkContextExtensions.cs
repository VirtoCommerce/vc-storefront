using System;
using System.IO;
using System.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class WorkContextExtensions
    {
        public static void SetCurrentPage(this WorkContext context, ContentPage page)
        {
            context.Layout = page.Layout;
            context.CurrentPage = page;
            context.CurrentPageSeo = new SeoInfo
            {
                Language = page.Language,
                MetaDescription = string.IsNullOrEmpty(page.Description) ? page.Title : page.Description,
                Title = page.Title,
                Slug = page.Permalink
            };
        }

        public static ContentPage FindContentPageByName(this WorkContext context, string name)
        {
            return context.Pages
                .OfType<ContentPage>()
                .Where(x => string.Equals(x.Url, name, StringComparison.OrdinalIgnoreCase) && Path.GetExtension(x.FileName) == ".page")
                .FindWithLanguage(context.CurrentLanguage);
        }
    }
}
