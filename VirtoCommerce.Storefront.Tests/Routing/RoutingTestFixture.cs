using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using VirtoCommerce.Storefront.Tests.Routing.Infrastructure;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Routing
{
    public abstract class RoutingTestFixture : IClassFixture<WebApplicationFactory<Startup>>, IDisposable
    {
        public RoutingTestFixture(WebApplicationFactory<Startup> factory)
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

        protected HttpClient Client { get; }

        protected static string BuildStorefrontControllerMethodName(string controllerName, string methodName)
        {
            return $"VirtoCommerce.Storefront.Controllers.{controllerName}Controller.{methodName} " +
                   "(VirtoCommerce.Storefront)";
        }

        protected static string BuildStorefrontApiControllerMethodName(string controllerName, string methodName)
        {
            return $"VirtoCommerce.Storefront.Controllers.Api.{controllerName}Controller.{methodName} " +
                   "(VirtoCommerce.Storefront)";
        }

        protected async Task<RoutingDataResult> AssertThatGetRequestExecutesSuccessfully(string url)
        {
            var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var routingData = JsonConvert.DeserializeObject<RoutingDataResult>(responseContent);
            Assert.NotNull(routingData);

            return routingData;
        }

        protected async Task<RoutingDataResult> AssertThatPostRequestExecutesSuccessfully(string url)
        {
            var response = await Client.PostAsync(url, null);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var routingData = JsonConvert.DeserializeObject<RoutingDataResult>(responseContent);
            Assert.NotNull(routingData);

            return routingData;
        }

        protected void AssertThatControllerNameAndActionMatchExpectedValues(RoutingDataResult routingData,
            string expectedControllerName, string expectedActionName)
        {
            var expectedMethodName = BuildStorefrontControllerMethodName(expectedControllerName, expectedActionName);
            Assert.Equal(expectedMethodName, routingData.ControllerMethodName);
        }

        protected void AssertThatApiControllerNameAndActionMatchExpectedValues(RoutingDataResult routingData,
            string expectedControllerName, string expectedActionName)
        {
            var expectedMethodName = BuildStorefrontApiControllerMethodName(expectedControllerName, expectedActionName);
            Assert.Equal(expectedMethodName, routingData.ControllerMethodName);
        }

        protected void AssertThatArgumentsAreEmpty(RoutingDataResult routingData)
        {
            Assert.Empty(routingData.Arguments);
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
