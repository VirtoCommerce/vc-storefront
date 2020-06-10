using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);
            var source = await response.Content?.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}
