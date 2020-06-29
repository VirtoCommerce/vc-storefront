using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Stores;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Routing
{
    [Trait("Category", "CI")]
    public class PathStringExtensionsTests
    {
        [Theory]
        [InlineData("/store", "/store")]
        [InlineData("/store/path", "/store/path")]
        [InlineData("/", "/")]
        [InlineData("", "")]
        public void TrimStorePath_NoStoreUrls_DoesNothing(string pathToHandle, string expectedPath)
        {
            // Arrange
            var storeStub = new Store();

            // Act
            var trimmedPath = new PathString(pathToHandle).TrimStorePath(storeStub);

            // Assert
            Assert.Equal(expectedPath, trimmedPath);
        }

        [Theory]
        [InlineData("/store", "/store")]
        [InlineData("/store/path", "/store/path")]
        [InlineData("/", "/")]
        [InlineData("", "")]
        public void TrimStorePath_NoPathInStoreUrl_DoesNothing(string pathToHandle, string expectedPath)
        {
            // Arrange
            var storeStub = new Store() { Url = "http://localhost/" };

            // Act
            var trimmedPath = new PathString(pathToHandle).TrimStorePath(storeStub);

            // Assert
            Assert.Equal(expectedPath, trimmedPath);
        }

        [Theory]
        [InlineData("/store", "/store")]
        [InlineData("/store/path", "/store/path")]
        [InlineData("/", "/")]
        [InlineData("", "")]
        public void TrimStorePath_NoPathInStoreSecureUrl_DoesNothing(string pathToHandle, string expectedPath)
        {
            // Arrange
            var storeStub = new Store() { SecureUrl = "http://localhost/" };

            // Act
            var trimmedPath = new PathString(pathToHandle).TrimStorePath(storeStub);

            // Assert
            Assert.Equal(expectedPath, trimmedPath);
        }

        [Theory]
        [InlineData("/store/Electronics", "/Electronics")]
        [InlineData("/Electronics", "/Electronics")]
        [InlineData("/store/store/Electronics", "/store/Electronics")]
        [InlineData("/account/store/Electronics", "/account/store/Electronics")]
        public void TrimStorePath_PathInStoreUrl_TrimsFirstOccurrenceFromBeginning(string pathToHandle, string expectedPath)
        {
            // Arrange
            var storeStub = new Store() { Url = "http://localhost/store" };

            // Act
            var trimmedPath = new PathString(pathToHandle).TrimStorePath(storeStub);

            // Assert
            Assert.Equal(expectedPath, trimmedPath);
        }

        [Theory]
        [InlineData("/store/Electronics", "/Electronics")]
        [InlineData("/Electronics", "/Electronics")]
        [InlineData("/store/store/Electronics", "/store/Electronics")]
        [InlineData("/account/store/Electronics", "/account/store/Electronics")]
        public void TrimStorePath_PathInStoreSecureUrl_TrimsFirstOccurrenceFromBeginning(string pathToHandle, string expectedPath)
        {
            // Arrange
            var storeStub = new Store() { SecureUrl = "http://localhost/store" };

            // Act
            var trimmedPath = new PathString(pathToHandle).TrimStorePath(storeStub);

            // Assert
            Assert.Equal(expectedPath, trimmedPath);
        }
    }
}
