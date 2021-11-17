using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Main working context contains all data which could be used in presentation logic
    /// </summary>
    public class WorkContext : IDisposable, ICloneable
    {
        public WorkContext()
        {
            ExternalLoginProviders = new List<LoginProvider>();
            ApplicationSettings = new Dictionary<string, object>();
            Form = new Form();
        }

        public SlugRoutingData SlugRoutingData { get; set; }

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

        public IDictionary<string, string> QueryString { get; set; }

        public IMutablePagedList<Country> Countries { get; set; }

        //Contains  the collections of breadcrumbs are relevant to  the current request
        public IMutablePagedList<Breadcrumb> Breadcrumbs { get; set; }
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
        /// Current store
        /// </summary>
        public Store CurrentStore { get; set; }
        public Store Shop => CurrentStore;


        /// <summary>
        /// Current single form aggregates ModelState errors
        /// </summary>
        public Form Form { get; set; }

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

        /// <summary>
        /// List of all b2b roles
        /// </summary>
        public IEnumerable<Role> BusinessToBusinessRoles { get; set; }

        public string ErrorMessage { get; set; }


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
        /// Gets or sets the dictionary of application settings
        /// </summary>
        public IDictionary<string, object> ApplicationSettings { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; } = 1;
        /// <summary>
        /// Current page size
        /// </summary>
        public int PageSize { get; set; } = 20;

        public int SkipCount
        {
            get
            {
                return (PageNumber - 1) * PageSize;
            }
        }
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
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // cleanup
            }
        }

        public object Clone()
        {
            var result = MemberwiseClone() as WorkContext;
            return result;
        }

        #endregion
    }
}
