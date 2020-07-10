using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain.ImageProcessing
{
    public class ImagekitImageProcessor : IImageProcessor
    {
        private readonly string _serviceUrl;

        public ImagekitImageProcessor(IOptions<ImageProcessorOptions> options)
        {
            _serviceUrl = options.Value.ServiceUrl.TrimEnd('/');
        }
        public string ResolveUrl(string inputUrl, int width, int height)
        {
            string tmpUrl = inputUrl.TrimStart('/');
            if (tmpUrl.StartsWith("http"))
            {
                tmpUrl = tmpUrl.Replace("http://", "").Replace("https://", "");
                string[] urlParts = tmpUrl.Split('/');
                tmpUrl = string.Join("/", urlParts.Skip(1));
            }
            string result = string.Join("/", _serviceUrl, tmpUrl);
            if (width > 0 && height > 0)
            {
                result += $"?tr=w-{width},h-{height}";
            }
            return result;
        }
    }
}
