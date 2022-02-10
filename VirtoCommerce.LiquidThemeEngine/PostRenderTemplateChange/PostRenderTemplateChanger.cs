using System.Collections.Generic;
using VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations;

namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange
{
    public class PostRenderTemplateChanger : IPostRenderTemplateChange
    {
        private readonly IList<IPostRenderTemplateChangeOperation> _operations = new[]
        {
            new ExternalLinksPostRenderTemplateOperation()
        };

        public IList<IPostRenderTemplateChangeOperation> Operations => _operations;

        public void Change(ref string renderResult)
        {
            foreach (var operation in Operations)
            {
                operation.Run(ref renderResult);
            }
        }
    }
}
