using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Tests.Routing.Infrastructure;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Routing
{
    public class RoutingTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        public RoutingTests(WebApplicationFactory<Startup> factory)
        {
            Client = factory
                .WithWebHostBuilder(
                    builder => builder.ConfigureServices(
                        services => services.AddMvc(
                            options => options.Filters.Add<RoutingTestingActionFilter>()
                        )
                    )
                ).CreateClient();
        }

        public HttpClient Client { get; }

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
