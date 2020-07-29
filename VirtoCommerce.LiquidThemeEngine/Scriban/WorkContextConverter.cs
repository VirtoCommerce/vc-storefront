using System;
using Scriban.Runtime;
using VirtoCommerce.LiquidThemeEngine.Filters;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Scriban
{
    public static class WorkContextConverter
    {
        public static ScriptObject ToScriptObject(this WorkContext workContext)
        {
            var scriptObject = new ScriptObject();
            scriptObject.Import(workContext);

            scriptObject.Import(typeof(CommonFilters));
            scriptObject.Import(typeof(CommerceFilters));
            scriptObject.Import(typeof(TranslationFilter));
            scriptObject.Import(typeof(UrlFilters));
            scriptObject.Import(typeof(MoneyFilters));
            scriptObject.Import(typeof(HtmlFilters));
            scriptObject.Import(typeof(StringFilters));
            scriptObject.Import(typeof(ArrayFilters));
            scriptObject.Import(typeof(MathFilters));
            scriptObject.Import(typeof(StandardFilters));
            scriptObject.Import(typeof(FeatureFilter));

            //TODO: blank isn't same as was in previous version now it is only represents a null check, need to find out solution or replace in themes == blank check to to .empty? == false expression
            scriptObject.SetValue("blank", EmptyScriptObject.Default, true);
            //Store special layout setter action in the context, it is allows to set the WorkContext.Layout property from template during rendering in the CommonFilters.Layout function
            Action<string> layoutSetter = (layout) => workContext.Layout = layout;
            scriptObject.Add("layout_setter", layoutSetter);
            return scriptObject;

        }
    }
}
