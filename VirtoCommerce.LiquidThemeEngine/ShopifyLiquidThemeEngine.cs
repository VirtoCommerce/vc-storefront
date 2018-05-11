using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using DotLiquid;
using DotLiquid.Exceptions;
using DotLiquid.FileSystems;
using DotLiquid.ViewEngine.Exceptions;
using LibSassHost;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.LiquidThemeEngine.Converters;
using VirtoCommerce.LiquidThemeEngine.Extensions;
using VirtoCommerce.LiquidThemeEngine.Filters;
using VirtoCommerce.LiquidThemeEngine.Operators;
using VirtoCommerce.LiquidThemeEngine.Tags;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.LiquidThemeEngine
{
    /// <summary>
    /// Shopify compliant theme folder structure and all methods for rendering
    /// assets - storages for css, images and other assets
    /// config - contains theme configuration
    /// layout - master pages and layouts
    /// locales - localization resources
    /// snippets - snippets - partial views
    /// templates - view templates
    /// </summary>
    public class ShopifyLiquidThemeEngine : IFileSystem, ILiquidThemeEngine
    {
        private readonly LiquidThemeEngineOptions _options;
        private static readonly Regex _isLiquid = new Regex("[{}|]", RegexOptions.Compiled);      
        private const string _liquidTemplateFormat = "{0}.liquid";
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStorefrontUrlBuilder _storeFrontUrlBuilder;
        private readonly IMemoryCache _memoryCache;
        private readonly IContentBlobProvider _themeBlobProvider;

        static ShopifyLiquidThemeEngine()
        {
            Liquid.UseRubyDateFormat = true;
            // Register custom tags (Only need to do this once)
            Template.RegisterFilter(typeof(CommonFilters));
            Template.RegisterFilter(typeof(CommerceFilters));
            Template.RegisterFilter(typeof(TranslationFilter));
            Template.RegisterFilter(typeof(UrlFilters));
            Template.RegisterFilter(typeof(DateFilters));
            Template.RegisterFilter(typeof(MoneyFilters));
            Template.RegisterFilter(typeof(HtmlFilters));
            Template.RegisterFilter(typeof(StringFilters));
            Template.RegisterFilter(typeof(ArrayFilters));
            Template.RegisterFilter(typeof(MathFilters));

            Condition.Operators["contains"] = CommonOperators.ContainsMethod;

            Template.RegisterTag<AntiforgeryTag>("anti_forgery");
            Template.RegisterTag<LayoutTag>("layout");
            Template.RegisterTag<FormTag>("form");
            Template.RegisterTag<PaginateTag>("paginate");
        }

        public ShopifyLiquidThemeEngine(IMemoryCache memoryCache, IWorkContextAccessor workContextAccessor,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStorefrontUrlBuilder storeFrontUrlBuilder, IContentBlobProvider contentBlobProvder, IOptions<LiquidThemeEngineOptions> options)
        {
            _workContextAccessor = workContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _storeFrontUrlBuilder = storeFrontUrlBuilder;
            _options = options.Value;
            _memoryCache = memoryCache;
            _themeBlobProvider = contentBlobProvder;
        }

        /// <summary>
        /// Main work context
        /// </summary>
        public WorkContext WorkContext => _workContextAccessor.WorkContext;

        /// <summary>
        /// Current HttpContext
        /// </summary>
        public HttpContext HttpContext => _httpContextAccessor.HttpContext;

        /// <summary>
        /// Store url builder
        /// </summary>
        public IStorefrontUrlBuilder UrlBuilder => _storeFrontUrlBuilder;

        /// <summary>
        /// Default master view name
        /// </summary>
        public string MasterViewName => _options.DefaultLayout;

        /// <summary>
        /// Current theme name
        /// </summary>
        public string CurrentThemeName => !string.IsNullOrEmpty(WorkContext.CurrentStore.ThemeName) ? WorkContext.CurrentStore.ThemeName : "default";

        public string CurrentThemeSettingPath => Path.Combine(CurrentThemePath, "config", "settings_data.json");
        public string CurrentThemeLocalePath => Path.Combine(CurrentThemePath, "locales");
        /// <summary>
        /// Current theme base path
        /// </summary>
        private string CurrentThemePath => Path.Combine("Themes", WorkContext.CurrentStore.Id, CurrentThemeName);

        #region IFileSystem members
        public string ReadTemplateFile(Context context, string templateName)
        {
            return ReadTemplateByName(templateName);
        }
        #endregion

        #region ILiquidThemeEngine Members
        public IEnumerable<string> DiscoveryPaths
        {
            get
            {
                IEnumerable<string> retVal = Enumerable.Empty<string>();
                if (WorkContext.CurrentStore != null)
                {
                    retVal = _options.TemplatesDiscoveryFolders.Select(x => Path.Combine(CurrentThemePath, x));
                }
                return retVal;
            }
        }

        /// <summary>
        /// Return stream for requested  asset file  (used for search current and base themes assets)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Stream GetAssetStream(string filePath)
        {
            Stream retVal = null;
            var filePathWithoutExtension = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
            //file.ext => file.ext || file || file.liquid || file.ext.liquid        
            var searchPatterns = new[] { filePath, filePathWithoutExtension, string.Format(_liquidTemplateFormat, filePathWithoutExtension), string.Format(_liquidTemplateFormat, filePath) };

            string currentThemeFilePath = null;
            //try to search in current store theme 
            if (_themeBlobProvider.PathExists(Path.Combine(CurrentThemePath, "assets")))
            {
                currentThemeFilePath = searchPatterns.SelectMany(x => _themeBlobProvider.Search(Path.Combine(CurrentThemePath, "assets"), x, true)).FirstOrDefault();
            }

            if (currentThemeFilePath != null)
            {
                retVal = _themeBlobProvider.OpenRead(currentThemeFilePath);
                filePath = currentThemeFilePath;
            }

            if (retVal != null && filePath.EndsWith(".liquid"))
            {
                var shopifyContext = WorkContext.ToShopifyModel(UrlBuilder);
                var parameters = shopifyContext.ToLiquid() as Dictionary<string, object>;
                var settings = GetSettings("''");
                parameters.Add("settings", settings);

                var templateContent = retVal.ReadToString();
                retVal.Dispose();

                var template = RenderTemplate(templateContent, parameters);
                retVal = new MemoryStream(Encoding.UTF8.GetBytes(template));
            }

            if (retVal != null && (filePath.Contains(".scss.") && filePath.EndsWith(".liquid") || filePath.EndsWith(".scss")))
            {
                var content = retVal.ReadToString();
                retVal.Dispose();

                try
                {
                    //handle scss resources
                    CompilationResult result = SassCompiler.Compile(content);
                    content = result.CompiledContent;

                    retVal = new MemoryStream(Encoding.UTF8.GetBytes(content));
                }
                catch (Exception ex)
                {
                    throw new SaasCompileException(filePath, content, ex);
                }
            }

            return retVal;
        }
        

        /// <summary>
        /// Return hash of requested asset (used for file versioning)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string GetAssetHash(string filePath)
        {
            var cacheKey = CacheKey.With(GetType(), "GetAssetHash", filePath);
            return _memoryCache.GetOrCreate(cacheKey,  (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(new CompositeChangeToken(new[] { ThemeEngineCacheRegion.CreateChangeToken(), _themeBlobProvider.Watch(filePath) }));

                using (var stream = GetAssetStream(filePath))
                {
                    var hashAlgorithm = CryptoConfig.AllowOnlyFipsAlgorithms ? (SHA256)new SHA256CryptoServiceProvider() : new SHA256Managed();
                    return WebEncoders.Base64UrlEncode(hashAlgorithm.ComputeHash(stream));
                }
            });
        }

        /// <summary>
        /// resolve  template path by it name
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public string ResolveTemplatePath(string templateName)
        {
            if (WorkContext.CurrentStore == null)
                return null;

            var liquidTemplateFileName = string.Format(_liquidTemplateFormat, templateName);
            var curentThemeDiscoveryPaths = _options.TemplatesDiscoveryFolders.Select(x => Path.Combine(CurrentThemePath, x, liquidTemplateFileName));

            //Try to find template in current theme folder
            return curentThemeDiscoveryPaths.FirstOrDefault(x => _themeBlobProvider.PathExists(x));
        }

        /// <summary>
        /// Render template by name and with passed context (parameters)
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string RenderTemplateByName(string templateName, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(templateName))
                throw new ArgumentNullException(nameof(templateName));

            var templateContent = ReadTemplateByName(templateName);
            var retVal = RenderTemplate(templateContent, parameters);
            return retVal;
        }

        /// <summary>
        /// Render template by content and parameters
        /// </summary>
        /// <param name="templateContent"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string RenderTemplate(string templateContent, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(templateContent))
            {
                return templateContent;
            }

            var isLiquidTemplate = _isLiquid.Match(templateContent);
            if (!isLiquidTemplate.Success)
            {
                return templateContent;
            }

            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }

            Template.FileSystem = this;
            //TODO:       
            var renderParams = new RenderParameters
            {
                LocalVariables = Hash.FromDictionary(parameters),
                RethrowErrors = _options.RethrowLiquidRenderErrors
            };

            var parsedTemplate = Template.Parse(templateContent);

            var retVal = parsedTemplate.RenderWithTracing(renderParams);

            //Copy key values which were generated in rendering to out parameters
            if (parsedTemplate.Registers != null)
            {
                foreach (var registerPair in parsedTemplate.Registers)
                {
                    parameters[registerPair.Key] = registerPair.Value;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Read shopify theme settings from 'config' folder
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public IDictionary GetSettings(string defaultValue = null)
        {
            var cacheKey = CacheKey.With(GetType(), "GetSettings", CurrentThemeSettingPath,  defaultValue);
            return _memoryCache.GetOrCreate(cacheKey,  (cacheItem) =>
            {
                cacheItem.AddExpirationToken(new CompositeChangeToken(new[] { ThemeEngineCacheRegion.CreateChangeToken(), _themeBlobProvider.Watch(CurrentThemeSettingPath) }));
                var retVal = new DefaultableDictionary(defaultValue);
                //Load all data from current theme config
                var resultSettings = InnerGetAllSettings(_themeBlobProvider, CurrentThemeSettingPath);
                if (resultSettings != null)
                {
                    //Get actual preset from merged config
                    var currentPreset = resultSettings.GetValue("current");
                    if (currentPreset is JValue)
                    {
                        var currentPresetName = ((JValue) currentPreset).Value.ToString();
                        var presets = resultSettings.GetValue("presets") as JObject;
                        if (presets == null || !presets.Children().Any())
                        {
                            throw new StorefrontException("Setting presets not defined");
                        }

                        IList<JProperty> allPresets = presets.Children().Cast<JProperty>().ToList();
                        resultSettings = allPresets.FirstOrDefault(p => p.Name == currentPresetName).Value as JObject;
                        if (resultSettings == null)
                        {
                            throw new StorefrontException($"Setting preset with name '{currentPresetName}' not found");
                        }
                    }
                    if (currentPreset is JObject)
                    {
                        resultSettings = (JObject) currentPreset;
                    }

                    var dict = resultSettings.ToObject<Dictionary<string, object>>().ToDictionary(x => x.Key, x => x.Value);
                    retVal = new DefaultableDictionary(dict, defaultValue);
                }

                return retVal;
            });
        }


        /// <summary>
        /// Read localization resources 
        /// </summary>
        /// <returns></returns>
        public JObject ReadLocalization()
        {
            var cacheKey = CacheKey.With(GetType(), "ReadLocalization", CurrentThemeLocalePath, WorkContext.CurrentLanguage.CultureName);
            return _memoryCache.GetOrCreate(cacheKey, (cacheItem) =>
            {
                cacheItem.AddExpirationToken(new CompositeChangeToken(new[] { ThemeEngineCacheRegion.CreateChangeToken(), _themeBlobProvider.Watch(CurrentThemeLocalePath + "/*") }));
                return InnerReadLocalization(_themeBlobProvider, CurrentThemeLocalePath, WorkContext.CurrentLanguage) ?? new JObject();
            });
        }

        /// <summary>
        /// Get relative url for assets (assets folder)
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public string GetAssetAbsoluteUrl(string assetName)
        {
            return UrlBuilder.ToAppAbsolute(_options.ThemesAssetsRelativeUrl.TrimEnd('/') + "/" + assetName.TrimStart('/'), WorkContext.CurrentStore, WorkContext.CurrentLanguage);
        }
        
        #endregion

        private static JObject InnerReadLocalization(IContentBlobProvider themeBlobProvider, string localePath, Language language)
        {
            JObject retVal = null;

            if (themeBlobProvider.PathExists(localePath))
            {
                JObject localeJson = null;
                JObject defaultJson = null;

                foreach (var languageName in new[] { language.CultureName, language.TwoLetterLanguageName })
                {
                    var currentLocalePath = Path.Combine(localePath, string.Concat(languageName, ".json"));

                    if (themeBlobProvider.PathExists(currentLocalePath))
                    {
                        using (var stream = themeBlobProvider.OpenRead(currentLocalePath))
                        {
                            localeJson = JsonConvert.DeserializeObject<dynamic>(stream.ReadToString());
                        }
                        break;
                    }
                }

                var localeDefaultPath = themeBlobProvider.Search(localePath, "*.default.json", false).FirstOrDefault();

                if (localeDefaultPath != null && themeBlobProvider.PathExists(localeDefaultPath))
                {
                    using (var stream = themeBlobProvider.OpenRead(localeDefaultPath))
                    {
                        defaultJson = JsonConvert.DeserializeObject<dynamic>(stream.ReadToString());
                    }
                }

                //Need merge default and requested localization json to resulting object
                retVal = defaultJson ?? localeJson;

                if (defaultJson != null && localeJson != null)
                {
                    retVal.Merge(localeJson, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
                }
            }
            return retVal;
        }

        private static JObject InnerGetAllSettings(IContentBlobProvider themeBlobProvider, string settingsPath)
        {
            if(settingsPath == null)
            {
                throw new ArgumentNullException(nameof(settingsPath));
            }

            JObject retVal = null;
         
            if (themeBlobProvider.PathExists(settingsPath))
            {
                using (var stream = themeBlobProvider.OpenRead(settingsPath))
                {
                    retVal = JsonConvert.DeserializeObject<JObject>(stream.ReadToString());
                }
            }
            return retVal;
        }

        private string ReadTemplateByName(string templateName)
        {
            var templatePath = ResolveTemplatePath(templateName);
            if (string.IsNullOrEmpty(templatePath))
            {
                throw new FileSystemException($"The template '{templateName}' was not found. The following locations were searched:<br/>{string.Join("<br/>", DiscoveryPaths)}");
            }

            var cacheKey = CacheKey.With(GetType(), "ReadTemplateByName", templatePath);
            return _memoryCache.GetOrCreate(cacheKey, (cacheItem) =>
            {
                cacheItem.AddExpirationToken(new CompositeChangeToken(new[] { ThemeEngineCacheRegion.CreateChangeToken(), _themeBlobProvider.Watch(templatePath) }));
                using (var stream = _themeBlobProvider.OpenRead(templatePath))
                {
                    return stream.ReadToString();
                }
            });
        }
    }
}
