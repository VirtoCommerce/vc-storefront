using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Routing
{
    public class HomeControllerRoutingTests : RoutingTestFixture
    {
        public HomeControllerRoutingTests(WebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Theory]
        [InlineData("Electronics/en-US/")]
        [InlineData("Electronics/")]
        [InlineData("")]
        public async Task PrefixesTest(string url)
        {
            // Act & Assert
            var routingData = await AssertThatGetRequestExecutesSuccessfully(url);

            AssertThatControllerNameAndActionMatchExpectedValues(routingData, "Home", "Index");
            AssertThatArgumentsAreEmpty(routingData);
        }
    }
}
