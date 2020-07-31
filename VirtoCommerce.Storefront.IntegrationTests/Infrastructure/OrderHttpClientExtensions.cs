using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Domain;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class OrderHttpClientExtensions
    {
        //storefrontapi/cart/{name}/{type}/createorder?removeCart=true
        public static async Task<OrderCreatedInfo> CreateOrderFromCartAsync(this HttpClient client)
        {
            var content = new StringContent("{}",
                Encoding.UTF8,
                "application/json");

            var createResponse = await client.PostAsync($"{TestEnvironment.CartEndpoint}/createorder", content);
            var createSource = await createResponse.Content?.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<OrderCreatedInfo>(createSource);

            return result;
        }
    }
}
