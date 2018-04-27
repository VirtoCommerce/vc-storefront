using DotLiquid;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;
using storefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ShopifyContextStaticConverter
    {
        public static ShopifyThemeWorkContext ToShopifyModel(this storefrontModel.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidContext(workContext, urlBuilder);
        }
    }

    public partial class ShopifyModelConverter
    {
        private static readonly string[] _poweredLinks = {
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">.NET ecommerce platform</a> by Virto",
                                             "<a href=\"http://virtocommerce.com/shopping-cart\" rel=\"nofollow\" target=\"_blank\">Shopping Cart</a> by Virto",
                                             "<a href=\"http://virtocommerce.com/shopping-cart\" rel=\"nofollow\" target=\"_blank\">.NET Shopping Cart</a> by Virto",
                                             "<a href=\"http://virtocommerce.com/shopping-cart\" rel=\"nofollow\" target=\"_blank\">ASP.NET Shopping Cart</a> by Virto",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">.NET ecommerce</a> by Virto",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">.NET ecommerce framework</a> by Virto",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">ASP.NET ecommerce</a> by Virto Commerce",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">ASP.NET ecommerce platform</a> by Virto",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">ASP.NET ecommerce framework</a> by Virto",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">Enterprise ecommerce</a> by Virto",
                                             "<a href=\"http://virtocommerce.com\" rel=\"nofollow\" target=\"_blank\">Enterprise ecommerce platform</a> by Virto",
                                         };

        public virtual ShopifyThemeWorkContext ToLiquidContext(storefrontModel.WorkContext workContext, IStorefrontUrlBuilder urlBuilder)
        {
            var result = new ShopifyThemeWorkContext();


            result.CurrentPage = 1;
            result.Layout = workContext.Layout;
            result.CountryOptionTags = string.Join("\r\n", workContext.AllCountries.OrderBy(c => c.Name).Select(c => c.ToOptionTag()));
            result.PageDescription = workContext.CurrentPageSeo != null ? workContext.CurrentPageSeo.MetaDescription : string.Empty;
            result.PageTitle = workContext.CurrentPageSeo != null ? workContext.CurrentPageSeo.Title : string.Empty;
            result.PageImageUrl = workContext.CurrentPageSeo != null ? workContext.CurrentPageSeo.ImageUrl : string.Empty;
            result.CanonicalUrl = workContext.CurrentPageSeo != null ? urlBuilder.ToAppAbsolute(workContext.CurrentPageSeo.Slug) : null;
            result.Shop = workContext.CurrentStore != null ? ToLiquidShop(workContext.CurrentStore, workContext) : null;
            result.Cart = workContext.CurrentCart != null && workContext.CurrentCart.IsValueCreated ? ToLiquidCart(workContext.CurrentCart.Value, workContext.CurrentLanguage, urlBuilder) : null;
            result.Product = workContext.CurrentProduct != null ? ToLiquidProduct(workContext.CurrentProduct) : null;
            result.Vendor = workContext.CurrentVendor != null ? ToLiquidVendor(workContext.CurrentVendor) : null;
            result.Customer = workContext.CurrentUser != null && workContext.CurrentUser.IsRegisteredUser ? ToLiquidCustomer(workContext.CurrentUser, workContext, urlBuilder) : null;
            result.AllStores = workContext.AllStores.Select(x => ToLiquidShop(x, workContext)).ToArray();
            result.CurrentCurrency = workContext.CurrentCurrency != null ? ToLiquidCurrency(workContext.CurrentCurrency) : null;
            result.CurrentLanguage = workContext.CurrentLanguage != null ? ToLiquidLanguage(workContext.CurrentLanguage) : null;

            if (workContext.CurrentProductSearchCriteria != null && workContext.CurrentProductSearchCriteria.Terms.Any())
            {
                result.CurrentTags =
                    new TagCollection(
                        workContext.CurrentProductSearchCriteria.Terms.Select(t => ToLiquidTag(t)).ToList());
            }

            if (workContext.CurrentCategory != null)
            {
                result.Collection = ToLiquidCollection(workContext.CurrentCategory, workContext);
            }

            if (workContext.Categories != null)
            {
                result.Collections = new Collections(new MutablePagedList<Collection>((pageNumber, pageSize, sortInfos) =>
                {
                    workContext.Categories.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Collection>(workContext.Categories.Select(x => ToLiquidCollection(x, workContext)), workContext.Categories);
                }, 1, workContext.Categories.PageSize));
            }

            if (workContext.Products != null)
            {
                result.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos) =>
                {
                    workContext.Products.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Product>(workContext.Products.Select(x => ToLiquidProduct(x)), workContext.Products);
                }, workContext.Products.PageNumber, workContext.Products.PageSize);
            }

            if (workContext.Vendors != null)
            {
                result.Vendors = new MutablePagedList<Vendor>((pageNumber, pageSize, sortInfos) =>
                {
                    workContext.Vendors.Slice(pageNumber, pageSize, sortInfos);
                    return new StaticPagedList<Vendor>(workContext.Vendors.Select(x => ToLiquidVendor(x)), workContext.Vendors);
                }, workContext.Vendors.PageNumber, workContext.Vendors.PageSize);
            }

            if (workContext.CurrentProductSearchCriteria != null && !string.IsNullOrEmpty(workContext.CurrentProductSearchCriteria.Keyword) && workContext.Products != null)
            {
                result.Search = ToLiquidSearch(workContext.Products, workContext);
            }
            else if (workContext.CurrentStaticSearchCriteria != null && !string.IsNullOrEmpty(workContext.CurrentStaticSearchCriteria.Keyword))
            {
                result.Search = new Search
                {
                    Performed = true,
                    SearchIn = workContext.CurrentStaticSearchCriteria.SearchIn,
                    Terms = workContext.CurrentStaticSearchCriteria.Keyword
                };
                if (workContext.StaticContentSearchResult != null && workContext.StaticContentSearchResult.Any())
                {
                    result.Search.Results = new MutablePagedList<Drop>((pageNumber, pageSize, sortInfos) =>
                    {
                        var pagedContentItems = new MutablePagedList<ContentItem>(workContext.StaticContentSearchResult);
                        pagedContentItems.Slice(pageNumber, pageSize, sortInfos);
                        return new StaticPagedList<Drop>(workContext.StaticContentSearchResult.Select(x => ToLiquidPage(x)), pagedContentItems);
                    }, 1, workContext.StaticContentSearchResult.PageSize);
                }
            }

            if (workContext.CurrentLinkLists != null)
            {
                result.Linklists = new Linklists(workContext.CurrentLinkLists.Select(x => ToLiquidLinklist(x, workContext, urlBuilder)));
            }

            if (workContext.Pages != null)
            {
                result.Pages = new Pages(workContext.Pages.OfType<ContentPage>().Select(x => ToLiquidPage(x)));
                result.Blogs = new Blogs(workContext.Blogs.Select(x => ToLiquidBlog(x, workContext.CurrentLanguage)));
            }

            if (workContext.CurrentOrder != null)
            {
                result.Order = ToLiquidOrder(workContext.CurrentOrder, workContext.CurrentLanguage, urlBuilder);
            }

            if (workContext.CurrentQuoteRequest != null)
            {
                result.QuoteRequest = ToLiquidQuoteRequest(workContext.CurrentQuoteRequest.Value);
            }

            result.PaymentFormHtml = workContext.PaymentFormHtml;

            if (workContext.CurrentPage != null)
            {
                result.Page = ToLiquidPage(workContext.CurrentPage);
            }

            if (workContext.CurrentBlog != null)
            {
                result.Blog = ToLiquidBlog(workContext.CurrentBlog, workContext.CurrentLanguage);
            }
            if (workContext.CurrentBlogSearchCritera != null)
            {
                result.BlogSearch = ToLiquidBlogSearch(workContext.CurrentBlogSearchCritera);
            }

            if (workContext.CurrentBlogArticle != null)
            {
                result.Article = ToLiquidArticle(workContext.CurrentBlogArticle);
            }

            if(workContext.Form != null)
            {
                result.Form = new Form
                {
                    Properties = new Dictionary<string, object>()
                };
                var formProps = workContext.Form.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var formPropNames = formProps.Select(x => x.Name).ToArray();
                foreach (var property in formProps)
                {
                    var propertyValue = property.GetValue(workContext.Form);
                    if (propertyValue != null)
                    {
                        result.Form.Properties[Template.NamingConvention.GetMemberName(property.Name)] = propertyValue;
                        if (typeof(IEntity).IsAssignableFrom(property.PropertyType) || typeof(IValueObject).IsAssignableFrom(property.PropertyType))
                        {
                            //For it is user type need to register this type as Drop in Liquid Template
                            Template.RegisterSafeType(property.GetType(), formPropNames);
                            var allChildEntities = propertyValue.GetFlatObjectsListWithInterface<IEntity>();
                            foreach (var type in allChildEntities.Select(x => x.GetType()).Distinct())
                            {
                                Template.RegisterSafeType(type, formPropNames);            
                            }
                            var allChildLiquidObjects = propertyValue.GetFlatObjectsListWithInterface<IValueObject>();
                            foreach (var type in allChildLiquidObjects.Select(x => x.GetType()).Distinct())
                            {
                                Template.RegisterSafeType(type, formPropNames);
                            }
                        }
                    }
                }
            }

            if (workContext.StorefrontNotification != null)
            {
                result.Notification = ToLiquidNotification(workContext.StorefrontNotification);
            }

            result.ExternalLoginProviders = workContext.ExternalLoginProviders.Select(p => new LoginProvider
            {
                AuthenticationType = p.AuthenticationType,
                Caption = p.Caption,
                Properties = p.Properties
            }).ToList();

            result.ApplicationSettings = new MetafieldsCollection("application_settings", workContext.ApplicationSettings);

            //Powered by link
            if (workContext.CurrentStore != null)
            {
                var storeName = workContext.CurrentStore.Name;
                var hashCode = (uint)storeName.GetHashCode();
                result.PoweredByLink = _poweredLinks[hashCode % _poweredLinks.Length];
            }

            if (workContext.RequestUrl != null)
            {
                result.RequestUrl = workContext.RequestUrl.ToString();

                //Populate current page number
                result.CurrentPage = workContext.PageNumber ?? 1;
                result.PageSize = workContext.PageSize ?? 0;
            }

            if (workContext.AvailableRoles != null)
            {
                result.AvailableRoles = workContext.AvailableRoles.Select(x => new Role
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToArray();
            }

            if(workContext.CurrentFulfillmentCenter != null)
            {
                result.FulfillmentCenter = workContext.CurrentFulfillmentCenter.ToShopifyModel();
            }

            if(workContext.FulfillmentCenters != null)
            {
                result.FulfillmentCenters = new MutablePagedList<FulfillmentCenter>((pageNumber, pageSize, sortInfos) =>
                 {
                     workContext.FulfillmentCenters.Slice(pageNumber, pageSize, sortInfos);
                     return new StaticPagedList<FulfillmentCenter>(workContext.FulfillmentCenters.Select(x => x.ToShopifyModel()), workContext.FulfillmentCenters);
                 }, workContext.FulfillmentCenters.PageNumber, workContext.FulfillmentCenters.PageSize);
            }
            return result;
        }
    }
}
