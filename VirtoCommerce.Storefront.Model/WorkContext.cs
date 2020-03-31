using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Inventory;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Pricing;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Main working context contains all data which could be used in presentation logic
    /// </summary>
    public partial class WorkContext : IDisposable, ICloneable
    {
        public WorkContext()
        {
            ExternalLoginProviders = new List<LoginProvider>();
            ApplicationSettings = new Dictionary<string, object>();
            Form = new Form();
        }

        public string Template { get; set; }
        /// <summary>
        /// Merchants can specify a page_description.
        /// </summary>
        public string PageDescription => CurrentPageSeo?.MetaDescription ?? string.Empty;

        /// <summary>
        /// The liquid object page_title returns the title of the current page.
        /// </summary>
        public string PageTitle => CurrentPageSeo?.Title ?? string.Empty;
        public string PageImageUrl => CurrentPageSeo?.ImageUrl ?? string.Empty;
        public string CanonicalUrl => CurrentPageSeo?.Slug ?? string.Empty;

        /// <summary>
        /// Layout which will be used for rendering current view
        /// </summary>
        public string Layout { get; set; }
        /// <summary>
        /// Current request url example: http:/host/app/store/en-us/search?page=2
        /// </summary>
        public Uri RequestUrl { get; set; }

        public NameValueCollection QueryString { get; set; }

        public IMutablePagedList<Country> Countries { get; set; }

        /// <summary>
        /// Current user
        /// </summary>
        public User CurrentUser { get; set; }
        public User Customer => CurrentUser;

        /// <summary>
        /// Current language and culture
        /// </summary>
        public Language CurrentLanguage { get; set; }

        /// <summary>
        /// Current currency
        /// </summary>
        public Currency CurrentCurrency { get; set; }

        public SeoInfo CurrentPageSeo { get; set; }

        /// <summary>
        /// Represent current account orders search criteria taken from request url
        /// </summary>
        public OrderSearchCriteria CurrentOrderSearchCriteria { get; set; }

        /// <summary>
        /// Current store
        /// </summary>
        public Store CurrentStore { get; set; }
        public Store Shop => CurrentStore;

        /// <summary>
        /// Gets or sets the current shopping cart
        /// </summary>
        public Lazy<ShoppingCart> CurrentCart { get; set; }
        public ShoppingCart Cart => (CurrentCart?.IsValueCreated ?? false) ? CurrentCart.Value : null;


        /// <summary>
        /// Current single form aggregates ModelState errors
        /// </summary>
        public Form Form { get; set; }


        /// <summary>
        /// Represent current quotes search criteria taken from request url
        /// </summary>
        public QuoteSearchCriteria CurrentQuoteSearchCriteria { get; set; }

        public Lazy<QuoteRequest> CurrentQuoteRequest { get; set; }
        public QuoteRequest QuoteRequest => (CurrentQuoteRequest?.IsValueCreated ?? false) ? CurrentQuoteRequest.Value : null;

        /// <summary>
        /// Gets or sets the HTML code for payment method prepared form
        /// </summary>
        public string PaymentFormHtml { get; set; }

        /// <summary>
        /// Gets or sets the collection of site navigation menu link lists
        /// </summary>
        public IMutablePagedList<MenuLinkList> CurrentLinkLists { get; set; }
        [JsonIgnore]
        public IMutablePagedList<MenuLinkList> Linklists => CurrentLinkLists;

        /// <summary>
        /// List of all supported stores
        /// </summary>
        public IList<Store> AllStores { get; set; }

        /// <summary>
        /// List of all active system currencies
        /// </summary>
        public IList<Currency> AllCurrencies { get; set; }

        /// <summary>
        /// List of all available roles
        /// </summary>
        public IEnumerable<Role> AvailableRoles { get; set; }

        public string ErrorMessage { get; set; }
        /// <summary>
        /// List of active pricelists
        /// </summary>
        public IMutablePagedList<Pricelist> CurrentPricelists { get; set; }

        #region Catalog Properties

        /// <summary>
        /// Represent current product
        /// </summary>
        public Product CurrentProduct { get; set; }
        [JsonIgnore]
        public Product Product => CurrentProduct;
        /// <summary>
        /// Represent current category
        /// </summary>
        public Category CurrentCategory { get; set; }
        public Category Collection => CurrentCategory;

        /// <summary>
        /// Represent all store catalog categories filtered by current search criteria CurrentCatalogSearchCriteria (loaded on first access by lazy loading)
        /// </summary>
        public IMutablePagedList<Category> Categories { get; set; }
        public IMutablePagedList<Category> Collections => Categories;

        /// <summary>
        /// Represent products filtered by current search criteria CurrentCatalogSearchCriteria (loaded on first access by lazy loading)
        /// </summary>
        public IMutablePagedList<Product> Products { get; set; }

        /// <summary>
        /// Represent the current product search result
        /// </summary>
        public CatalogSearchResult ProductSearchResult { get; set; } = new CatalogSearchResult();
        /// <summary>
        /// Current search product search criterias
        /// </summary>
        public ProductSearchCriteria CurrentProductSearchCriteria { get; set; }

        /// <summary>
        /// Current product response group
        /// </summary>
        public ItemResponseGroup CurrentProductResponseGroup { get; set; }

        public IMutablePagedList<Vendor> Vendors { get; set; }

        public Vendor CurrentVendor { get; set; }
        public Vendor Vendor => CurrentVendor;

        #endregion

        #region Static Content Properties
        public ContentPage CurrentPage { get; set; }
        public ContentPage Page => CurrentPage;

        public StaticContentSearchCriteria CurrentStaticSearchCriteria { get; set; }

        public IMutablePagedList<ContentItem> StaticContentSearchResult { get; set; }

        public BlogSearchCriteria CurrentBlogSearchCritera { get; set; }

        public Blog CurrentBlog { get; set; }
        public Blog Blog => CurrentBlog;


        public BlogArticle CurrentBlogArticle { get; set; }
        public BlogArticle Article => CurrentBlogArticle;
        #endregion

        private DateTime? _utcNow;
        /// <summary>
        /// Represent current storefront datetime in UTC (may be changes to test in future or past events)
        /// </summary>
        public DateTime StorefrontUtcNow
        {
            get
            {
                return _utcNow ?? DateTime.UtcNow;
            }
            set
            {
                _utcNow = value;
            }
        }

        public IList<Country> AllCountries { get; set; }

        public CustomerOrder CurrentOrder { get; set; }
        public CustomerOrder Order => CurrentOrder;


        public StorefrontNotification StorefrontNotification { get; set; }
        public StorefrontNotification Notification => StorefrontNotification;

        /// <summary>
        /// All static content items (Pages, blog articles etc) for current store and theme
        /// </summary>
        public IMutablePagedList<ContentItem> Pages { get; set; }

        /// <summary>
        ///  All blogs with articles for current store and theme
        /// </summary>
        public IMutablePagedList<Blog> Blogs { get; set; }

        /// <summary>
        /// Gets or sets the collection of external login providers
        /// </summary>
        public IList<LoginProvider> ExternalLoginProviders { get; set; }

        /// <summary>
        /// Current fulfillment center
        /// </summary>
        public FulfillmentCenter CurrentFulfillmentCenter { get; set; }
        public FulfillmentCenter FulfillmentCenter => CurrentFulfillmentCenter;

        /// <summary>
        ///  All available fulfillment centers 
        /// </summary>
        public IMutablePagedList<FulfillmentCenter> FulfillmentCenters { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of application settings
        /// </summary>
        public IDictionary<string, object> ApplicationSettings { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int? PageNumber { get; set; } = 1;
        /// <summary>
        /// Current page size
        /// </summary>
        public int? PageSize { get; set; }
        /// <summary>
        /// Settings defined in theme
        /// </summary>
        public IDictionary<string, object> Settings { get; set; }

        public string Version { get; set; }

        #region GDPR consent
        public bool CanTrack { get; set; }
        public string ConsentCookie { get; set; }
        #endregion

        /// <summary>
        /// Checks if the current hosting environment name is Microsoft.AspNetCore.Hosting.EnvironmentName.Development.
        /// </summary>
        public bool IsDevelopment { get; set; }

        /// <summary>
        /// The flag that indicates that themes resources won't use cache to be able to preview changes frequently during development and designing.
        /// This setting will be helpful especially when the AzureBlobStorage provider is used because of unable to monitor blob changes as well as for file system
        /// </summary>
        public bool IsPreviewMode { get; set; }


        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public object Clone()
        {
            //TODO: Implement deep clone
            var result = MemberwiseClone() as WorkContext;
            return result;
        }

        #endregion
    }
}
