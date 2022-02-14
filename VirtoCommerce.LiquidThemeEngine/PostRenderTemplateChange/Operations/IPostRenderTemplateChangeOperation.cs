namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations
{
    public interface IPostRenderTemplateChangeOperation
    {
        string Run(string renderResult);
    }
}
