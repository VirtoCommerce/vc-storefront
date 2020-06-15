using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.IntegrationTests.Models;
using VirtoCommerce.Storefront.Model.Cart;

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

        public static async Task<T> PostAsync<T>(this HttpClient client, string url, HttpContent content)
        {
            var response = await client.PostAsync(url, content);
            var source = await response.Content?.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(source);
        }

        public static HttpClient Login(this HttpClient client, string userName, string password)
        {
            var loginPage = client.GetAsync("account/login").GetAwaiter().GetResult();

            var antiforgery = new AntiforgeryCookie(loginPage.Headers);
            var content = new MultipartFormDataContent
            {
                { new StringContent(userName), "customer[user_name]" },
                { new StringContent(password), "customer[password]" },
                { new StringContent(antiforgery.Value), "__RequestVerificationToken" }
            };

            var loginResponse = client.PostAsync("account/login", content).GetAwaiter().GetResult();
            if (!loginResponse.IsSuccessStatusCode && loginResponse.StatusCode != HttpStatusCode.Redirect)
            {
                throw new Exception($"Login failed: {loginResponse.StatusCode}");
            }

            var cookie = new AntiforgeryCookie(loginResponse.Headers);
            client.DefaultRequestHeaders.Add(cookie.Key, new[] { cookie.Value });

            return client;
        }

        public static HttpClient InsertCartItem(this HttpClient client, AddCartItem item)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(item),
                Encoding.UTF8,
                "application/json");

            var response = client.PostAsync(TestEnvironment.CartItemsEndpoint, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Fill cart failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient СlearCart(this HttpClient client)
        {
            var response = client.PostAsync(TestEnvironment.CartClearEndpoint, new StringContent("")).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Clear cart failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient Logout(this HttpClient client)
        {
            var response = client.GetAsync(TestEnvironment.LogoutEndpoint).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Logout failed: {response.StatusCode}");
            }

            return client;
        }

        //public static HttpClient Login(this HttpClient client)
        //{
        //    return (client as StorefrontClient);
        //}
    }
}
