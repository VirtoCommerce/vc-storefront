using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using VirtoCommerce.LiquidThemeEngine.Converters;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class DotLiquidThemedView : IView
    {
        private readonly ILiquidThemeEngine _liquidThemeEngine;
        private readonly string _viewName;
        private readonly bool _isMainPage;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IStorefrontUrlBuilder _urlBuilder;
        public DotLiquidThemedView(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ILiquidThemeEngine themeEngine, string viewName, string path, bool isMainPage)
        {
            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException(nameof(viewName));

            _workContextAccessor = workContextAccessor;
            _urlBuilder = urlBuilder;
            _liquidThemeEngine = themeEngine ?? throw new ArgumentNullException(nameof(themeEngine));
            _viewName = viewName;
            _isMainPage = isMainPage;
            Path = path;
        }

        public string Path { get; private set; }

        #region IView members

        public void Render(ViewContext viewContext, TextWriter writer)
        {
        }

        public async Task RenderAsync(ViewContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var shopifyContext = _workContextAccessor.WorkContext.ToShopifyModel(_urlBuilder);
            //Set current template
            shopifyContext.Template = _viewName;

            var formErrors = FormErrors.FromModelState(context.ViewData.ModelState);

            if (shopifyContext.Form == null)
            {
                //Set single Form object with errors for shopify compilance
                shopifyContext.Form = new Form();
            }
            shopifyContext.Form.PostedSuccessfully = !string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.InvariantCultureIgnoreCase);
            if (formErrors.Messages.Any())
            {
                shopifyContext.Form.Errors = formErrors;
                shopifyContext.Form.PostedSuccessfully = false;
            }

            //EU General Data Protection Regulation (GDPR) support 
            var consentFeature = context.HttpContext.Features.Get<ITrackingConsentFeature>();
            if (consentFeature != null)
            {
                shopifyContext.CanTrack = !consentFeature?.CanTrack ?? false;
                shopifyContext.ConsentCookie = consentFeature?.CreateConsentCookie();
            }

            // Copy data from the view context over to DotLiquid
            //var parameters = shopifyContext.ToLiquid() as Dictionary<string, object>;

            //Add settings to context
            shopifyContext.Settings = _liquidThemeEngine.GetSettings();
            //TODO:
            //foreach (var item in context.ViewData)
            //{
            //    //parameters.Add(Template.NamingConvention.GetMemberName(item.Key), item.Value);
            //    parameters.Add(item.Key, item.Value);
            //}
            //foreach (var item in context.TempData)
            //{
            //    //parameters.Add(Template.NamingConvention.GetMemberName(item.Key), item.Value);
            //    parameters.Add(item.Key, item.Value);
            //}

            if (!string.IsNullOrEmpty(_workContextAccessor.WorkContext.ErrorMessage))
            {
                shopifyContext.ErrorMessage = _workContextAccessor.WorkContext.ErrorMessage;
            }

            var viewTemplate = _liquidThemeEngine.RenderTemplateByName(_viewName, shopifyContext);

            // don't use layouts for partial views when masterViewName is not specified
            if (_isMainPage)
            {
                var masterViewName = shopifyContext.Layout ?? "theme";

                var headerTemplate = _liquidThemeEngine.RenderTemplateByName("content_header", shopifyContext);

                //add special placeholder 'content_for_layout' to content it will be replaced in master page by main content
                shopifyContext.ContentForLayout = viewTemplate;
                shopifyContext.ContentForHeader = headerTemplate;
                shopifyContext.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                viewTemplate = _liquidThemeEngine.RenderTemplateByName(masterViewName, shopifyContext);
            }
            await context.Writer.WriteAsync(viewTemplate);
        }

        #endregion
    }
}
