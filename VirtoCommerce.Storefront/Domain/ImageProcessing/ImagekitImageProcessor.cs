using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain.ImageProcessing
{
    public class ImagekitImageProcessor : IImageProcessor
    {
        private readonly string _serviceUrl;

        public ImagekitImageProcessor(IOptions<ImageProcessorOptions> options)
        {
            _serviceUrl = options.Value?.ServiceUrl?.TrimEnd('/');
        }
        public string ResolveUrl(string inputUrl, int? width, int? height)
        {
            if (string.IsNullOrEmpty(inputUrl))
            {
                return inputUrl;
            }
            string tmpUrl = inputUrl.TrimStart('/');
            if (tmpUrl.StartsWith("http"))
            {
                tmpUrl = tmpUrl.Replace("http://", "").Replace("https://", "");
                string[] urlParts = tmpUrl.Split('/');
                tmpUrl = string.Join("/", urlParts.Skip(1));
            }
            string result = string.Join("/", _serviceUrl, tmpUrl);
            if (width.HasValue || height.HasValue)
            {
                StringBuilder imageParam = new StringBuilder("?tr=");
                if (width.HasValue && width.Value > 0)
                {
                    imageParam.Append($"w-{width.Value},");
                }
                if (height.HasValue && height.Value > 0)
                {
                    imageParam.Append($"h-{height.Value}");
                }
                result += imageParam.ToString().TrimEnd(',');
            }
            return result;
        }
    }
}
