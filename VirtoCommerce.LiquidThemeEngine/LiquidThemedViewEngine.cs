using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class LiquidThemedViewEngine : ILiquidViewEngine
    {
        private readonly ILiquidThemeEngine _themeEngine;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IStorefrontUrlBuilder _urlBuilder;
        public LiquidThemedViewEngine(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ILiquidThemeEngine themeEngine)
        {
            _themeEngine = themeEngine;
            _workContextAccessor = workContextAccessor;
            _urlBuilder = urlBuilder;
        }

        #region IViewEngine members

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            return InnerGetView(viewName, isMainPage);
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            return InnerGetView(viewPath, isMainPage);
        }
        #endregion

        protected ViewEngineResult InnerGetView(string view, bool isMainPage)
        {
            var searchedLocations = Enumerable.Empty<string>();

            //Do not handle without a set WorkContext
            if (_workContextAccessor.WorkContext != null)
            {
                var path = _themeEngine.ResolveTemplatePath(view);
                if (!string.IsNullOrEmpty(path))
                {
                    return ViewEngineResult.Found(view, new LiquidThemedView(_workContextAccessor, _urlBuilder, _themeEngine, view, path, isMainPage));
                }
                searchedLocations = _themeEngine.DiscoveryPaths.ToArray();
            }
            return ViewEngineResult.NotFound(view, searchedLocations);
        }
    }
}
