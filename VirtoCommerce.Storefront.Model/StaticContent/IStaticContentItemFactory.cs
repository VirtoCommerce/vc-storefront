namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IStaticContentItemFactory
    {
        ContentItem GetItemFromPath(string path);
    }
}
