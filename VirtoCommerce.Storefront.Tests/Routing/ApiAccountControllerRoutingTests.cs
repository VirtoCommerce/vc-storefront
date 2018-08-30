using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Routing
{
    public class ApiAccountControllerRoutingTests : RoutingTestFixture
    {
        public ApiAccountControllerRoutingTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async void TestCallingGetCurrentUser()
        {
            // Act & Assert
            var routingData = await AssertThatGetRequestExecutesSuccessfully("storefrontapi/account");

            AssertThatApiControllerNameAndActionMatchExpectedValues(routingData, "ApiAccount", "GetCurrentUser");
            AssertThatArgumentsAreEmpty(routingData);
        }
    }
}
