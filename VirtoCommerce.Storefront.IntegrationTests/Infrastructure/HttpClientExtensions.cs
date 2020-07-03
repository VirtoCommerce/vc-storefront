using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.Storefront.IntegrationTests.Models;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Subscriptions;

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
            var loginPage = client.GetAsync(TestEnvironment.LoginEndpoint).GetAwaiter().GetResult();

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
            return SetAntiforgeryHeader(client, cookie);
        }

        public static HttpClient GotoMainPage(this HttpClient client)
        {
            var mainPage = client.GetAsync("").GetAwaiter().GetResult();
            if (!mainPage.IsSuccessStatusCode)
            {
                throw new Exception($"Main page loading failed: {mainPage.StatusCode}");
            }

            var antiforgery = new AntiforgeryCookie(mainPage.Headers);

            return SetAntiforgeryHeader(client, antiforgery);
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

        public static HttpClient DeleteCartItem(this HttpClient client, string lineItemId)
        {
            var response = client.DeleteAsync(TestEnvironment.DeleteCartItemEndpoint(lineItemId)).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Delete line item from cart failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient ChangeCartItemPrice(this HttpClient client, ChangeCartItemPrice priceItem)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(priceItem),
                Encoding.UTF8,
                "application/json");

            var response = client.PutAsync(TestEnvironment.ItemPriceEndpoint, content).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ChangeCartItemPrice failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient ClearCart(this HttpClient client)
        {
            var response = client.PostAsync(TestEnvironment.CartClearEndpoint, new StringContent("")).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Clear cart failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient AddCoupon(this HttpClient client, string couponCode)
        {
            var response = client.PostAsync(TestEnvironment.AddCouponEndpoint(couponCode), null).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Add coupon failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient RemoveCoupon(this HttpClient client, string couponCode)
        {
            var response = client.DeleteAsync(TestEnvironment.RemoveCouponEndpoint(couponCode)).GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Remove coupon failed: {response.StatusCode}");
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

            if (client.DefaultRequestHeaders.TryGetValues(AntiforgeryCookie.Name, out var tokens))
            {
                client.DefaultRequestHeaders.Remove(AntiforgeryCookie.Name);
            }

            return client;
        }

        public static HttpClient AddOrUpdateCartPaymentPlan(this HttpClient client, PaymentPlan paymentPlan)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(paymentPlan),
                Encoding.UTF8,
                "application/json");

            var response = client.PostAsync(TestEnvironment.CartPaymentPlanEndpoint, content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Add or update cart payment plan failed: {response.StatusCode}");
            }

            return client;
        }

        public static HttpClient DeleteCartPaymentPlan(this HttpClient client)
        {
            var response = client.DeleteAsync(TestEnvironment.CartPaymentPlanEndpoint).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Delete cart payment plan failed: {response.StatusCode}");
            }

            return client;
        }

        public static async Task<HttpResponseMessage> AddOrUpdateCartPayment(this HttpClient client, Payment payment)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var serializedObject = JsonConvert.SerializeObject(payment, serializerSettings);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            return await client.PostAsync(TestEnvironment.CartPaymentEndpoint, content);
        }

        public static async Task<HttpResponseMessage> AddOrUpdateCartShipment(this HttpClient client, Shipment shipment)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var serializedObject = JsonConvert.SerializeObject(shipment, serializerSettings);
            var content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            return await client.PostAsync(TestEnvironment.CartShipmentEndpoint, content);
        }

        public static async Task<string> GetCart(this HttpClient client)
        {
            var response = await client.GetAsync(TestEnvironment.CartEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get cart failed: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetCartAvailPaymentMethods(this HttpClient client)
        {
            var response = await client.GetAsync(TestEnvironment.PaymentMethodsEndpoint);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get cart available payment methods failed: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetCartShipmentAvailShippingMethods(this HttpClient client)
        {
            var response = await client.GetAsync(TestEnvironment.ShippingMethodsEndpoint("1"));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Get cart available shipping methods failed: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }


        private static HttpClient SetAntiforgeryHeader(HttpClient client, AntiforgeryCookie cookie)
        {
            if (client.DefaultRequestHeaders.TryGetValues(AntiforgeryCookie.Name, out var tokens))
            {
                client.DefaultRequestHeaders.Remove(AntiforgeryCookie.Name);
            }

            client.DefaultRequestHeaders.Add(AntiforgeryCookie.Name, new[] { cookie.Value });

            return client;
        }
    }
}
