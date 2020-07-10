using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Identity;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Security.Contracts;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class SecurityGraphQLProvider
    {
        private readonly IGraphQLClient _client;

        public SecurityGraphQLProvider(IGraphQLClient client)
        {
            _client = client;
        }

        #region User
        public async Task<IdentityResult> CreateUserAsync(User user)
        {
            var request = new GraphQLRequest
            {
                Query = this.CreateUserRequest(),
                Variables = new
                {
                    Command = new
                    {
                        user.UserName,
                        user.UserType,
                        user.Password,
                        user.Email,
                        user.StoreId,
                        memberId = !string.IsNullOrEmpty(user.ContactId) ? user.ContactId : user.Contact?.Id
                    }
                }
            };
            var response = await _client.SendMutationAsync<CreateUserResponseDto>(request);
            response.ThrowExceptionOnError();
            return response.Data?.SecurityResult.ToIdentityResult();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var request = new GraphQLRequest
            {
                Query = this.UpdateUserRequest(),
                Variables = new
                {
                    Command = new
                    {
                        user.UserName,
                        user.UserType,
                        user.Email,
                        user.Id,
                        memberId = !string.IsNullOrEmpty(user.ContactId) ? user.ContactId : user.Contact?.Id,
                        user.IsAdministrator,
                        user.LockoutEnabled,
                        LockoutEnd = user.LockoutEndDateUtc,
                        user.PhoneNumber,
                        user.PhoneNumberConfirmed,
                        //user.PhotoUrl,
                        roles = user.Roles.Select(x => new { x.Id, x.Name }),
                        user.TwoFactorEnabled,
                        user.PasswordHash,
                        user.SecurityStamp
                    }
                }
            };
            var response = await _client.SendMutationAsync<UpdateUserResponseDto>(request);
            response.ThrowExceptionOnError();
            return response.Data?.SecurityResult.ToIdentityResult();
        }

        public async Task DeleteUserAsync(string userName)
        {
            var request = new GraphQLRequest
            {
                Query = this.DeleteUserRequest(),
                Variables = new
                {
                    Command = new { userName }
                }
            };
            var response = await _client.SendMutationAsync<DeleteUserResponseDto>(request);
            response.ThrowExceptionOnError();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetUserByIdRequest(userId)
            };
            var response = await _client.SendQueryAsync<UserResponseDto>(request);

            return response.Data?.User;
        }

        public async Task<User> GetUserByNameAsync(string name)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetUserByNameRequest(name)
            };
            var response = await _client.SendQueryAsync<UserResponseDto>(request);

            return response.Data?.User;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetUserByEmailRequest(email)
            };
            var response = await _client.SendQueryAsync<UserResponseDto>(request);

            return response.Data?.User;
        }

        public async Task<User> GetUserByLoginAsync(string loginProvider, string providerKey)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetUserByLoginRequest(loginProvider, providerKey)
            };
            var response = await _client.SendQueryAsync<UserResponseDto>(request);

            return response.Data?.User;
        }

        #endregion User

        #region Role

        public async Task<IdentityResult> UpdateRoleAsync(Role role)
        {
            var request = new GraphQLRequest
            {
                Query = this.UpdateRoleRequest(),
                Variables = new
                {
                    Command = new
                    {
                        role
                    }
                }
            };
            var response = await _client.SendMutationAsync<UpdateRoleResponseDto>(request);
            response.ThrowExceptionOnError();
            return response.Data?.SecurityResult.ToIdentityResult();
        }

        public async Task<Role> GetRoleAsync(string roleId)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetRoleByIdRequest(roleId)
            };
            var response = await _client.SendQueryAsync<RoleResponseDto>(request);

            return response.Data?.Role;
        }

        #endregion Role 
    }
}
