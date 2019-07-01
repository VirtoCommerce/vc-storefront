using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.LiquidThemeEngine.Scriban;
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
        public virtual Task RenderAsync(ViewContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            return RenderInternalAsync(context);
        }

        #endregion

        protected virtual async Task RenderInternalAsync(ViewContext context)
        {
            var workContext = _workContextAccessor.WorkContext;
            //Set current template
            workContext.Template = _viewName;
            workContext.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var formErrors = context.ViewData.ModelState.Where(x => x.Value.Errors.Any())
                                                        .SelectMany(x => x.Value.Errors.Select(y => new FormError { Code = x.Key.PascalToKebabCase(), Description = y.ErrorMessage })).ToList();


            if (workContext.Form == null)
            {
                workContext.Form = new Form();
            }
            workContext.Form.PostedSuccessfully = !string.Equals(context.HttpContext.Request.Method, "GET", StringComparison.InvariantCultureIgnoreCase);
            if (formErrors.Any())
            {
                workContext.Form.Errors.AddRange(formErrors);
                workContext.Form.PostedSuccessfully = false;
            }

            //Add settings to context
            workContext.Settings = _liquidThemeEngine.GetSettings();

            if (string.IsNullOrEmpty(_workContextAccessor.WorkContext.ErrorMessage))
            {
                workContext.ErrorMessage = workContext.Form.Errors.FirstOrDefault()?.Description;
            }
            var scriptObject = workContext.ToScriptObject();

            var result = await _liquidThemeEngine.RenderTemplateByNameAsync(_viewName, scriptObject);

            // don't use layouts for partial views when masterViewName is not specified
            if (_isMainPage)
            {
                var masterViewName = workContext.Layout ?? "theme";

                var headerTemplate = await _liquidThemeEngine.RenderTemplateByNameAsync("content_header", scriptObject);

                //add special placeholder 'content_for_layout' to content it will be replaced in master page by main content
                scriptObject.Add("content_for_layout", result);
                scriptObject.Add("content_for_header", headerTemplate);

                result = await _liquidThemeEngine.RenderTemplateByNameAsync(masterViewName, scriptObject);
            }
            await context.Writer.WriteAsync(result);
        }

    }
}
