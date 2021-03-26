namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IImageProcessor
    {
        string ResolveUrl(string inputUrl, int? width, int? height);
    }
}
