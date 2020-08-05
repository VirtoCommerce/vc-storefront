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
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Security;
using Xunit;

namespace VirtoCommerce.Storefront.IntegrationTests.Tests
{
    [Trait("Category", "IntegrationTest")]
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
        public async Task RegisterUser_Registered()
        {
            //Arrange
            _client.Login("admin", "store");

            var request = CreateUserRegistration();

            //Act
            var result = await RegisterUserAsync(request);

            //Assert
            result.Should().BeTrue();
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

            _client.Login(request.UserName, request.Password).GotoMainPage();
            var organization = await GetCurrentOrganization();
            _client.Logout();
            
            _client.Login("admin", "store");
            organization.Name += "after_update";
            var content = new StringContent(
                JsonConvert.SerializeObject(organization),
                Encoding.UTF8,
                "application/json");

            //Act
            var response = await _client.PutAsync(TestEnvironment.OrganizationEndpoint, content);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            _client.Logout();
        }

        [Fact]
        public async Task UpdateAccount_UpdateUserName_UpdatedAndCheckRelationship()
        {
            //Arrange
            _client.Login("admin", "store");

            var request = CreateUserRegistration();
            await RegisterUserAsync(request);
            _client.Logout();

            _client.Login(request.UserName, request.Password).GotoMainPage();
            var user = await GetCurrentUser();
            user.UserName += "after_update";
            var content = new StringContent(
                JsonConvert.SerializeObject(new UserUpdateInfo
                {
                    FullName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Roles = user.Roles.Select(x => x.Name).ToList()
                }),
                Encoding.UTF8,
                "application/json");

            //Act
            var response = await _client.PostAsync(TestEnvironment.UserEndpoint, content);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
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
            Assert.Equal(addresses.FirstOrDefault().City, user.Addresses.FirstOrDefault().City);
            Assert.Equal(addresses.FirstOrDefault().CountryCode, user.Addresses.FirstOrDefault().CountryCode);
            Assert.Equal(addresses.FirstOrDefault().CountryName, user.Addresses.FirstOrDefault().CountryName);
            Assert.Equal(addresses.FirstOrDefault().PostalCode, user.Addresses.FirstOrDefault().PostalCode);
            Assert.Equal(addresses.FirstOrDefault().RegionId, user.Addresses.FirstOrDefault().RegionId);
            Assert.Equal(addresses.FirstOrDefault().Line1, user.Addresses.FirstOrDefault().Line1);
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

        private async Task<bool> RegisterUserAsync(OrganizationUserRegistration organization)
        {
            var body = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("customer[user_name]", organization.UserName),
                new KeyValuePair<string, string>("customer[password]", organization.Password),
                new KeyValuePair<string, string>("customer[first_name]", organization.FirstName),
                new KeyValuePair<string, string>("customer[last_name]", organization.LastName),
                new KeyValuePair<string, string>("customer[email]", organization.Email),
                new KeyValuePair<string, string>("customer[name]", organization.Name)                
            };
            var content = new FormUrlEncodedContent(body);
            var createResponse = await _client.PostAsync("account/register", content);

            return createResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }

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

        private OrganizationRegistration CreateOrganizationRegistration()
        {
            var ticks = (DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

            return new OrganizationRegistration
            {
                UserName = $"TestOrg{ticks}",
                FirstName = $"firstName{ticks}",
                LastName = $"lastName{ticks}",
                Password = "Somepass123!",
                Email = $"org{ticks}@org.com1",
                OrganizationName = $"OrgName{ticks}",
                StoreId = "B2B-store",
                Address = new Address { City = "Los Angeles", CountryCode = "USA", CountryName = "United States", PostalCode = "34535", RegionId = "CA", Line1 = "20945 Devonshire St Suite 102" },
            };
        }
    }
}
