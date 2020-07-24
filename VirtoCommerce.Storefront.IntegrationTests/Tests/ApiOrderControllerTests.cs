using System;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Tests
{
    [Trait("Category", "CI")]
    [CollectionDefinition("ApiOrderControllerTests", DisableParallelization = true)]
    public class ApiOrderControllerTests : IClassFixture<StorefrontApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private bool _isDisposed;

        public const string Login = "test@test.com";
        public const string Password = "!Password1";

        public ApiOrderControllerTests(StorefrontApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Disposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _client?.Dispose();
            }

            _isDisposed = true;
        }

        ~ApiOrderControllerTests()
        {
            Dispose(false);
        }

        #endregion Disposable

        public async Task GetCustomerOrder_CreateAndReturn_ReturnOrder()
        {

        }
    }
}
