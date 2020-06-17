using System.Drawing;
using System.IO;
using System.Net;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public static class StructuredDataFilters
    {
        public static string ImageMarkup(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;

            string result;
            var imageData = new WebClient().DownloadData(url);
            var imgStream = new MemoryStream(imageData);
            using (var img = Image.FromStream(imgStream))
            {
                result = "\"image\": {\n" +
                             "\"@type\": \"ImageObject\",\n" +
                             $"\"url\": \"{url}\",\n" +
                             $"\"width\": {img.Width},\n" +
                             $"\"height\": {img.Height}\n" +
                             "},\n";
            }

            return result;
        }
    }
}
