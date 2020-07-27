using System;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;
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
            //Arrange
            await _client.RegisterUserAsync(CreateUserRegistration());



            _client.Logout();
        }


        private OrganizationUserRegistration CreateUserRegistration()
        {
            var ticks = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

            return new OrganizationUserRegistration
            {
                UserName = $"TestUser{ticks}",
                FirstName = $"firstName{ticks}",
                LastName = $"lastName{ticks}",
                Password = "Somepass123!",
                Email = $"user{ticks}@us.com",
                Address = new Address { City = "Los Angeles", CountryCode = "USA", CountryName = "United States", PostalCode = "34535", RegionId = "CA", Line1 = "20945 Devonshire St Suite 102" },
            };
        }
    }
}
