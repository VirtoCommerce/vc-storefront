using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.IntegrationTests.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Security;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Tests
{
    [Trait("Category", "CI")]
    [CollectionDefinition("ApiAccountControlleTests", DisableParallelization = true)]
    public class ApiAccountControllerTests : IClassFixture<StorefrontApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private bool _isDisposed;

        public ApiAccountControllerTests(StorefrontApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact(Skip = "TODO: need to check permissions")]
        public async Task SearchOrganizationUsersAsync_GetOrganization()
        {
            //Arrange
            _client.Login("admin", "store");
            var request = CreateOrganizationRegistration();
            await RegisterOrganizationAsync(request);
            _client.Logout();
            _client.Login(request.UserName, request.Password);

            OrganizationContactsSearchCriteria searchCriteria = new OrganizationContactsSearchCriteria { PageSize = 1};
            var content = new StringContent(
                JsonConvert.SerializeObject(searchCriteria),
                Encoding.UTF8,
                "application/json");

            //Act
            var response = await _client.PostAsync($"{TestEnvironment.OrganizationEndpoint}/users/search", content);
            var source = await response.Content?.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<UserSearchResult>(source);

            //Assert
            result.Should().BeEquivalentTo(new UserSearchResult());
            _client.Logout();
        }

        [Fact]
        public async Task RegisterOrganization_Registered()
        {
            //Arrange
            _client.Login("admin", "store");

            var request = CreateOrganizationRegistration();

            //Act
            var result = await RegisterOrganizationAsync(request);

            //Assert
            result.Succeeded.Should().BeTrue();
            _client.Logout();
        }

        [Fact(Skip = "TODO: need to check permissions")]
        public async Task UpdateOrganization_Updated()
        {
            //Arrange
            
            _client.Login("admin", "store");

            var request = CreateOrganizationRegistration();
            await RegisterOrganizationAsync(request);
            _client.Logout();
            _client.Login(request.UserName, request.Password);
            
            _client.Logout();
            var organization = await GetCurrentOrganization();
            _client.Login("admin", "store");
            organization.Name += "after_update";
            var content = new StringContent(
                JsonConvert.SerializeObject(organization),
                Encoding.UTF8,
                "application/json");

            //Act
            await _client.PutAsync(TestEnvironment.OrganizationEndpoint, content);

            //Assert
            _client.Logout();
        }

        [Fact]
        public async Task UpdateAddresses_Updated()
        {
            //Arrange
            _client.Login("admin", "store");
            var request = CreateOrganizationRegistration();
            await RegisterOrganizationAsync(request);
            _client.Logout();

            _client.Login(request.UserName, request.Password);
            var addresses = new List<Address>() { new Address { City = "Washington", CountryCode = "USA", CountryName = "United States", PostalCode = "34535", RegionId = "DC", Line1 = "1111" } };
            var content = new StringContent(
                JsonConvert.SerializeObject(addresses),
                Encoding.UTF8,
                "application/json");

            //Act
            await _client.PostAsync("storefrontapi/account/addresses", content);

            //Assert
            var user = await GetCurrentUser();
            Assert.Equal(addresses.FirstOrDefault(), user.Addresses.FirstOrDefault());
            _client.Logout();
        }
        
        #region Dispose
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

        ~ApiAccountControllerTests()
        {
            Dispose(false);
        }

        #endregion Dispose


        private async Task<UserActionIdentityResult> RegisterOrganizationAsync(OrganizationRegistration organization)
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(organization),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync(TestEnvironment.OrganizationEndpoint, content);
            var createSource = await createResponse.Content?.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<UserActionIdentityResult>(createSource);

            return result;
        }

        private async Task<Organization> GetCurrentOrganization()
        {
            var response = await _client.GetAsync($"{TestEnvironment.OrganizationEndpoint}/current");
            var source = await response.Content?.ReadAsStringAsync();
            var excludedSource = JToken.Parse(source).RemovePropertyInChildren(new[] { "$" }, new[] { "dynamicProperties" }).ToString();
            var organization = JsonConvert.DeserializeObject<Organization>(excludedSource);

            return organization;
        }

        private async Task<User> GetCurrentUser()
        {
            var response = await _client.GetAsync("storefrontapi/account");
            var source = await response.Content?.ReadAsStringAsync();
            var excludedSource = JToken.Parse(source).RemovePropertyInChildren(new[] { "$", "$.organization", "$.contact", "$.contact.organization" }, new[] { "dynamicProperties" }).ToString();
            var user = JsonConvert.DeserializeObject<User>(excludedSource);

            return user;
        }

        private OrganizationRegistration CreateOrganizationRegistration()
        {
            var ticks = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

            return new OrganizationRegistration
            {
                UserName = $"TestOrg{ticks}",
                Password = "Somepass123!",
                Email = $"org{ticks}@org.com1",
                OrganizationName = $"OrgName{ticks}",
                Address = new Address { City = "Los Angeles", CountryCode = "USA", CountryName = "United States", PostalCode = "34535", RegionId = "CA", Line1 = "20945 Devonshire St Suite 102" },
            };
        }
    }
}
