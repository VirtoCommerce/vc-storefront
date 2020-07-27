using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class AccountHttpClientExtensions
    {
        public static async Task<bool> RegisterUserAsync(this HttpClient client, OrganizationUserRegistration organization)
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
            var createResponse = await client.PostAsync("account/register", content);

            return createResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
