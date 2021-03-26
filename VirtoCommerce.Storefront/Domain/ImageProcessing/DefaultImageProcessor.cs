using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain.ImageProcessing
{
    public class DefaultImageProcessor : IImageProcessor
    {
        public string ResolveUrl(string inputUrl, int? width, int? height)
        {
            return inputUrl;
        }
    }
}
