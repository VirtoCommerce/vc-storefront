using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Extensions;
using System.Security.Principal;
using VirtoCommerce.Storefront.Model.Customer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using VirtoCommerce.Storefront.Model.LinkList.Services;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Pricing.Services;
using VirtoCommerce.Storefront.Converters;

namespace VirtoCommerce.Storefront.Data.Stores
{
    public partial class WorkContextBuilder
    {
        private WorkContextBuilder(WorkContext workContext, HttpContext httpContext)
        {
            WorkContext = workContext;
            HttpContext = httpContext;
        }

        public static WorkContextBuilder FromHttpContext(HttpContext httpContext)
        {
            var result = new WorkContextBuilder(new WorkContext(), httpContext);
            result.WorkContext.RequestUrl = httpContext.Request.GetUri();
            var qs = result.WorkContext.QueryString = httpContext.Request.Query.ToNameValueCollection();
            result.WorkContext.PageNumber = qs["page"].ToNullableInt();
            result.WorkContext.PageSize = qs["count"].ToNullableInt();
            if(result.WorkContext.PageSize == null)
            {
                result.WorkContext.PageSize = qs["page_size"].ToNullableInt();
            }
            return result;
        }

        protected HttpContext HttpContext { get; }
        protected WorkContext WorkContext { get; }

        public WorkContext Build()
        {
            return WorkContext;
        }

        public WorkContextBuilder WithCountries(Country[] countries)
        {
            WorkContext.AllCountries = countries ?? throw new ArgumentNullException(nameof(countries));
            return this;
        }

        public WorkContextBuilder WithCountries(ICountriesService countryService)
        {
            if (countryService == null)
            {
                throw new ArgumentNullException(nameof(countryService));
            }

            return WithCountries(countryService.GetCountries().ToArray());
        }

        public WorkContextBuilder WithStores(Store[] stores, string defaultStoreId)
        {
            if (stores.IsNullOrEmpty())
            {
                throw new NoStoresException();
            }

            WorkContext.AllStores = stores;
            //Set current store
            //Try first find by store url (if it defined)
            var currentStore = stores.FirstOrDefault(x => HttpContext.Request.Path.StartsWithSegments(new PathString("/" + x.Id)));
            if (currentStore == null)
            {
                currentStore = stores.Where(x => x.IsStoreUrl(WorkContext.RequestUrl)).FirstOrDefault();
            }
            if (currentStore == null && defaultStoreId != null)
            {
                currentStore = stores.FirstOrDefault(x => x.Id.EqualsInvariant(defaultStoreId));
            }

            WorkContext.CurrentStore = currentStore ?? stores.FirstOrDefault();          

            //TODO: need to use DDD Specification for this conditions
            //Set current language
            var availLanguages = stores.SelectMany(s => s.Languages)
                                  .Union(stores.Select(s => s.DefaultLanguage))
                                  .Select(x => x.CultureName).Distinct().ToArray();

            //Try to get language from request
            var currentLanguage = WorkContext.CurrentStore.DefaultLanguage; 
            var regexpPattern = string.Format(@"\/({0})\/?", string.Join("|", availLanguages));
            var match = Regex.Match(HttpContext.Request.Path, regexpPattern, RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 1)
            {
                var language = new Language(match.Groups[1].Value);
                //Get store default language if language not in the supported by stores list
                currentLanguage = WorkContext.CurrentStore.Languages.Contains(language) ? language : currentLanguage;
            }
            WorkContext.CurrentLanguage = currentLanguage;
            return this;
        }

        public async Task<WorkContextBuilder> WithStoresAsync(IStoreService storeService, string defaultStoreId)
        {
            if (storeService == null)
            {
                throw new ArgumentNullException(nameof(storeService));
            }
            var stores = await storeService.GetAllStoresAsync();
            return WithStores(stores, defaultStoreId);
        }


        public WorkContextBuilder WithCurrencies(Currency[] currencies)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }
            if (WorkContext.AllStores == null)
            {
                throw new StorefrontException("Unable to set currencies without stores");
            }
            if (WorkContext.CurrentLanguage == null)
            {
                throw new StorefrontException("Unable to set currencies without language");
            }
            if (WorkContext.CurrentStore == null)
            {
                throw new StorefrontException("Unable to set currencies without current store");
            }

            WorkContext.AllCurrencies = currencies;

            //Sync store currencies with avail in system
            foreach (var store in WorkContext.AllStores)
            {
                store.SyncCurrencies(WorkContext.AllCurrencies, WorkContext.CurrentLanguage);
                store.CurrentSeoInfo = store.SeoInfos.FirstOrDefault(x => x.Language == WorkContext.CurrentLanguage);
            }

            //Set current currency
            //Get currency from request url  
            StringValues currencyCode;
            if (!HttpContext.Request.Query.TryGetValue("currency", out currencyCode))
            {
                //Next try get from Cookies
                currencyCode = HttpContext.Request.Cookies[StorefrontConstants.CurrencyCookie];
            }
            var currentCurrency = WorkContext.CurrentStore.DefaultCurrency;
            //Get store default currency if currency not in the supported by stores list
            if (!string.IsNullOrEmpty(currencyCode))
            {
                currentCurrency = WorkContext.CurrentStore.Currencies.FirstOrDefault(x => x.Equals(currencyCode)) ?? currentCurrency;
            }
            WorkContext.CurrentCurrency = currentCurrency;
            return this;
        }

        public async Task<WorkContextBuilder> WithCurrenciesAsync(ICurrencyService currencyService)
        {
            if (currencyService == null)
            {
                throw new ArgumentNullException(nameof(currencyService));
            }
            if (WorkContext.CurrentLanguage == null)
            {
                throw new StorefrontException("Unable to set currencies without language");
            }
            var currencies = await currencyService.GetAllCurrenciesAsync(WorkContext.CurrentLanguage);
            return WithCurrencies(currencies);
        }

        public Task<WorkContextBuilder> WithCatalogsAsync(ICatalogService catalogService)
        {
            //Initialize catalog search criteria
            WorkContext.CurrentProductSearchCriteria = new ProductSearchCriteria(WorkContext.CurrentLanguage, WorkContext.CurrentCurrency, WorkContext.QueryString);

            //Initialize product response group. Exclude properties meta-information for performance reason (property values will be returned)
            WorkContext.CurrentProductResponseGroup = EnumUtility.SafeParse(WorkContext.QueryString.Get("resp_group"), ItemResponseGroup.ItemLarge & ~ItemResponseGroup.ItemProperties);

            //This line make delay categories loading initialization (categories can be evaluated on view rendering time)
            WorkContext.Categories = new MutablePagedList<Category>((pageNumber, pageSize, sortInfos) =>
            {
                var criteria = new CategorySearchCriteria(WorkContext.CurrentLanguage)
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    ResponseGroup = CategoryResponseGroup.Small
                };

                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                var result = catalogService.SearchCategories(criteria);
                foreach (var category in result)
                {
                    category.Products = new MutablePagedList<Product>((pageNumber2, pageSize2, sortInfos2) =>
                    {
                        var productSearchCriteria = new ProductSearchCriteria(WorkContext.CurrentLanguage, WorkContext.CurrentCurrency)
                        {
                            PageNumber = pageNumber2,
                            PageSize = pageSize2,
                            Outline = category.Outline,
                            ResponseGroup = WorkContext.CurrentProductSearchCriteria.ResponseGroup
                        };

                        //criteria.CategoryId = category.Id;
                        if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos2.IsNullOrEmpty())
                        {
                            productSearchCriteria.SortBy = SortInfo.ToString(sortInfos2);
                        }

                        var searchResult = catalogService.SearchProducts(productSearchCriteria);

                        //Because catalog search products returns also aggregations we can use it to populate workContext using C# closure
                        //now workContext.Aggregation will be contains preloaded aggregations for current category
                        WorkContext.Aggregations = new MutablePagedList<Aggregation>(searchResult.Aggregations);
                        return searchResult.Products;
                    }, 1, ProductSearchCriteria.DefaultPageSize);
                }
                return result;
            }, 1, CategorySearchCriteria.DefaultPageSize);

            //This line make delay products loading initialization (products can be evaluated on view rendering time)
            WorkContext.Products = new MutablePagedList<Product>((pageNumber, pageSize, sortInfos) =>
            {
                var criteria = WorkContext.CurrentProductSearchCriteria.Clone();
                criteria.PageNumber = pageNumber;
                criteria.PageSize = pageSize;
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                var result = catalogService.SearchProducts(criteria);
                //Prevent double api request for get aggregations
                //Because catalog search products returns also aggregations we can use it to populate workContext using C# closure
                //now workContext.Aggregation will be contains preloaded aggregations for current search criteria
                WorkContext.Aggregations = new MutablePagedList<Aggregation>(result.Aggregations);
                return result.Products;
            }, 1, ProductSearchCriteria.DefaultPageSize);

            //This line make delay aggregation loading initialization (aggregation can be evaluated on view rendering time)
            WorkContext.Aggregations = new MutablePagedList<Aggregation>((pageNumber, pageSize, sortInfos) =>
            {
                var criteria = WorkContext.CurrentProductSearchCriteria.Clone();
                criteria.PageNumber = pageNumber;
                criteria.PageSize = pageSize;
                if (string.IsNullOrEmpty(criteria.SortBy) && !sortInfos.IsNullOrEmpty())
                {
                    criteria.SortBy = SortInfo.ToString(sortInfos);
                }
                //Force to load products and its also populate workContext.Aggregations by preloaded values
                WorkContext.Products.Slice(pageNumber, pageSize, sortInfos);
                return WorkContext.Aggregations;
            }, 1, ProductSearchCriteria.DefaultPageSize);

            return Task.FromResult(this);
        }

        public async Task<WorkContextBuilder> WithStaticContentAsync(IMenuLinkListService linkListService, IStaticContentService staticContentService)
        {
            if (WorkContext.CurrentStore == null)
            {
                throw new StorefrontException("Unable to set static content without store");
            }
            var linkLists =  await linkListService.LoadAllStoreLinkListsAsync(WorkContext.CurrentStore.Id);
            WorkContext.CurrentLinkLists = linkLists.GroupBy(x => x.Name).Select(x => x.FindWithLanguage(WorkContext.CurrentLanguage)).Where(x => x != null).ToList();

            // load all static content
            var allContentItems = staticContentService.LoadStoreStaticContent(WorkContext.CurrentStore);
            var blogs = allContentItems.OfType<Blog>().ToArray();
            var blogArticlesGroup = allContentItems.OfType<BlogArticle>().GroupBy(x => x.BlogName, x => x).ToList();
            foreach (var blog in blogs)
            {
                var blogArticles = blogArticlesGroup.FirstOrDefault(x => string.Equals(x.Key, blog.Name, StringComparison.OrdinalIgnoreCase));
                if (blogArticles != null)
                {
                    blog.Articles = new MutablePagedList<BlogArticle>(blogArticles);
                }
            }
            //TODO: performance and DDD specification instead if statements
            WorkContext.Pages = new MutablePagedList<ContentItem>(allContentItems.Where(x => x.Language.IsInvariant || x.Language == WorkContext.CurrentLanguage));
            WorkContext.Blogs = new MutablePagedList<Blog>(blogs.Where(x => x.Language.IsInvariant || x.Language == WorkContext.CurrentLanguage));

            // Initialize blogs search criteria 
            WorkContext.CurrentBlogSearchCritera = new BlogSearchCriteria(WorkContext.QueryString);

            return this;
        }

        public async Task<WorkContextBuilder> WithPrices(IPricingService pricingService)
        {
            WorkContext.CurrentPricelists = (await pricingService.EvaluatePricesListsAsync(WorkContext.ToPriceEvaluationContext())).ToList();
            return this;

        }
    }
}
