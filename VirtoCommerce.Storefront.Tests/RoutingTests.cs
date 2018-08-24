using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests
{
    public class RoutingTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        public RoutingTests(WebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        public HttpClient Client { get; }

        [InlineData("Electronics/en-US/")]
        [InlineData("Electronics/")]
        [InlineData("")]
        public async Task PrefixesTest(string url)
        {
            var response = await Client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("storefrontapi/account")]
        [InlineData("storefrontapi/account/someUserId")]
        [InlineData("storefrontapi/account/organization/current")]
        public async Task RoutingApiAccountGetTest(string url)
        {
            var response = await Client.GetAsync(url);

            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("storefrontapi/account")]
        [InlineData("storefrontapi/account/password")]
        [InlineData("storefrontapi/account/addresses")]
        [InlineData("storefrontapi/account/organization/users/search")]
        [InlineData("storefrontapi/account/invitation")]
        [InlineData("storefrontapi/account/organization")]
        [InlineData("storefrontapi/account/user")]
        [InlineData("storefrontapi/account/someUserId/lock")]
        [InlineData("storefrontapi/account/someUserId/unlock")]
        public async Task RoutingApiAccountPostTest(string url)
        {
            var response = await Client.PostAsync(url, content: null);

            response.EnsureSuccessStatusCode();
        }

        // put
        //[InlineData("storefrontapi/account/organization")]

        // delete
        //[InlineData("storefrontapi/account/someUserId")]

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}
