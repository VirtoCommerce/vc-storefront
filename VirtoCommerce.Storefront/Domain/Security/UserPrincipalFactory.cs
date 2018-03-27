using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class UserPrincipalFactory : IUserClaimsPrincipalFactory<User>
    {
        private readonly IdentityOptions _options;

        public UserPrincipalFactory(IOptions<IdentityOptions> optionsAccessor)
        {
            _options = optionsAccessor?.Value ?? new IdentityOptions();
        }

        public Task<ClaimsPrincipal> CreateAsync(User user)
        {
            //Create first anonymous identity
            var identity = new ClaimsIdentity();
            if(user.IsRegisteredUser)
            {
                //https://stackoverflow.com/questions/45261732/user-identity-isauthenticated-always-false-in-net-core-custom-authentication
                identity = new ClaimsIdentity("Registered");
            }

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            if (user.IsAdministrator)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, SecurityConstants.Roles.Administrator));
            }

            if (user.SelectedCurrencyCode != null)
            {
                identity.AddClaim(new Claim(SecurityConstants.Claims.CurrencyClaimType, user.SelectedCurrencyCode));
            }

            if (!user.AllowedStores.IsNullOrEmpty())
            {
                identity.AddClaim(new Claim(SecurityConstants.Claims.AllowedStoresClaimType, string.Join(",", user.AllowedStores)));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserName))
            {
                identity.AddClaim(new Claim(SecurityConstants.Claims.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserId))
            {
                identity.AddClaim(new Claim(SecurityConstants.Claims.OperatorUserIdClaimType, user.OperatorUserId));
                identity.AddClaim(new Claim(SecurityConstants.Claims.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if(!user.Permissions.IsNullOrEmpty())
            {
                foreach(var permission in user.Permissions)
                {
                    identity.AddClaim(new Claim(SecurityConstants.Claims.PermissionClaimType, permission));
                }
            }
            if (!user.Roles.IsNullOrEmpty())
            {
                foreach (var role in user.Roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
            }
            var principal = new ClaimsPrincipal(identity);

            return Task.FromResult(principal);
        }
    }
}
