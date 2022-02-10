namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations
{
    public interface IPostRenderTemplateChangeOperation
    {
        void Run(ref string renderResult);
    }
}
