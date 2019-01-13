using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class LiquidThemedView : IView
    {
        private readonly ILiquidThemeEngine _liquidThemeEngine;
        private readonly string _viewName;
        private readonly bool _isMainPage;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IStorefrontUrlBuilder _urlBuilder;
        public LiquidThemedView(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ILiquidThemeEngine themeEngine, string viewName, string path, bool isMainPage)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }
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
            {
                throw new ArgumentNullException(nameof(context));
            }

            var workContext = _workContextAccessor.WorkContext;
            //Set current template
            workContext.Template = _viewName;

            var formErrors = context.ViewData.ModelState.Where(x => x.Value.Errors.Any())
                                                        .SelectMany(x => x.Value.Errors.Select(y => y.ErrorMessage)).ToList();

            if (workContext.Form == null)
            {
                workContext.Form = new Form();
            }
            workContext.Form.PostedSuccessfully = !string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.InvariantCultureIgnoreCase);
            if (formErrors.Any())
            {
                workContext.Form.Errors = formErrors;
                workContext.Form.PostedSuccessfully = false;
            }

            // Copy data from the view context over to DotLiquid
            //var parameters = shopifyContext.ToLiquid() as Dictionary<string, object>;

            //Add settings to context
            workContext.Settings = _liquidThemeEngine.GetSettings();
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
                workContext.ErrorMessage = _workContextAccessor.WorkContext.ErrorMessage;
            }

            var viewTemplate = _liquidThemeEngine.RenderTemplateByName(_viewName, workContext);

            // don't use layouts for partial views when masterViewName is not specified
            if (_isMainPage)
            {
                var masterViewName = workContext.Layout ?? "theme";

                var headerTemplate = _liquidThemeEngine.RenderTemplateByName("content_header", workContext);

                //add special placeholder 'content_for_layout' to content it will be replaced in master page by main content
                workContext.ContentForLayout = viewTemplate;
                workContext.ContentForHeader = headerTemplate;
                workContext.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                viewTemplate = _liquidThemeEngine.RenderTemplateByName(masterViewName, workContext);
            }
            await context.Writer.WriteAsync(viewTemplate);
        }

        #endregion
    }
}
