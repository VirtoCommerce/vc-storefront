using System.Collections.Generic;
using VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations;

namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange
{
    public interface IPostRenderTemplateChange
    {
        IList<IPostRenderTemplateChangeOperation> Operations { get; }
        string Change(string renderResult);
    }
}
