using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Domain.Feedback
{
    public class HttpFeedbackItemService : IFeedbackItemService<FeedbackItem, (HttpStatusCode StatusCode, string Content)>
    {
        public async Task<(HttpStatusCode StatusCode, string Content)> SendAsync(FeedbackItem item)
        {
            var requestParams = string.Join('&', item.Parameters);
            using (var client = new HttpClient())
            {
                var bytes = Encoding.Default.GetBytes(requestParams);

                using (var stream = new MemoryStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                    using (var requestMessage = new HttpRequestMessage())
                    {
                        requestMessage.Method = new HttpMethod(item.HttpMethod ?? "GET");
                        requestMessage.RequestUri = new Uri(item.Url + (item.Url.Contains('?') ? '&' : '?') + requestParams);
                        requestMessage.Content = new StreamContent(stream);
                        using (var responseMessage = await client.SendAsync(requestMessage))
                        {
                            var content = await responseMessage.Content.ReadAsStringAsync();
                            return (responseMessage.StatusCode, content);
                        }
                    }
                }
            }
        }
    }
}
