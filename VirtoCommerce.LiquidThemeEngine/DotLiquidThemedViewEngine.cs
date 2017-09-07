using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Linq;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class DotLiquidThemedViewEngine : IViewEngine
    {
        private ShopifyLiquidThemeEngine _themeEngine;

        public DotLiquidThemedViewEngine(ShopifyLiquidThemeEngine themeEngine)
        {
            _themeEngine = themeEngine;
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
            var path = _themeEngine.ResolveTemplatePath(view);
            if (!string.IsNullOrEmpty(path))
            {
                return ViewEngineResult.Found(view, new DotLiquidThemedView(_themeEngine, view, path, isMainPage));
            }

            return ViewEngineResult.NotFound(view, _themeEngine.DiscoveryPaths.ToArray());
        }
    }
}
