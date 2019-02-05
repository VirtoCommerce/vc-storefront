using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Moq;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Model.Stores;
using Xunit;
using Language = VirtoCommerce.Storefront.Model.Language;

namespace VirtoCommerce.Storefront.Tests.OutputCache
{
    [Trait("Category", "CI")]
    public class ResponseCachingTests : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public ResponseCachingTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            Client = factory
                .WithWebHostBuilder(
                    builder => builder.ConfigureTestServices(services =>
                    {
                        var mockEngine = new Mock<ILiquidThemeEngine>();
                        mockEngine.Setup(x => x.ReadLocalization()).Returns(new Newtonsoft.Json.Linq.JObject());
                        mockEngine.Setup(x => x.GetAssetStream(It.IsAny<string>())).Returns(new MemoryStream(Encoding.UTF8.GetBytes("some data")));
                        mockEngine.Setup(x => x.ResolveTemplatePath(It.IsIn(new[] { "index" }))).Returns("path");
                        services.AddSingleton(mockEngine.Object);
                        var mockStoreService = new Mock<IStoreService>();
                        mockStoreService.Setup(x => x.GetAllStoresAsync()).ReturnsAsync(new Store[] { new Store { Id = "Electronics", DefaultLanguage = new Language("en-US") } });
                        services.AddSingleton(mockStoreService.Object);

                    })
                ).CreateClient();
            //Make initial request to get anonymous cookies
            var initialRequest = Client.GetAsync("/Electronics/en-US/common/notheme").GetAwaiter().GetResult();
        }
        public HttpClient Client { get; }


        [Theory]
        [InlineData("/Electronics/en-US/themes/localization.json")]
        [InlineData("/Electronics/en-US/themes/assets/shop.js")]
        public async Task ServesThemesAssets(string url)
        {

            // Act & Assert
            var initialResponse = await Client.GetAsync(url);
            var subsequentResponse = await Client.GetAsync(url);

            await AssertCachedResponseAsync(initialResponse, subsequentResponse);

        }

        private static async Task AssertCachedResponseAsync(HttpResponseMessage initialResponse, HttpResponseMessage subsequentResponse)
        {
            initialResponse.EnsureSuccessStatusCode();
            subsequentResponse.EnsureSuccessStatusCode();

            foreach (var header in initialResponse.Headers)
            {
                Assert.Equal(initialResponse.Headers.GetValues(header.Key), subsequentResponse.Headers.GetValues(header.Key));
            }
            Assert.True(subsequentResponse.Headers.Contains(HeaderNames.Age));
            Assert.Equal(await initialResponse.Content.ReadAsStringAsync(), await subsequentResponse.Content.ReadAsStringAsync());
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }

}
