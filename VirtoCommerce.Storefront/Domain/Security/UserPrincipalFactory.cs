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
    public class UserPrincipalFactory : UserClaimsPrincipalFactory<User>
    {

        public UserPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor)
            :base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var result = await base.GenerateClaimsAsync(user);
                     
            if (user.IsAdministrator)
            {
                result.AddClaim(new Claim(ClaimTypes.Role, SecurityConstants.Roles.Administrator));
            }

            if (user.SelectedCurrencyCode != null)
            {
                result.AddClaim(new Claim(SecurityConstants.Claims.CurrencyClaimType, user.SelectedCurrencyCode));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserName))
            {
                result.AddClaim(new Claim(SecurityConstants.Claims.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if (!string.IsNullOrEmpty(user.OperatorUserId))
            {
                result.AddClaim(new Claim(SecurityConstants.Claims.OperatorUserIdClaimType, user.OperatorUserId));
                result.AddClaim(new Claim(SecurityConstants.Claims.OperatorUserNameClaimType, user.OperatorUserName));
            }

            if (!user.Permissions.IsNullOrEmpty())
            {
                foreach (var permission in user.Permissions)
                {
                    result.AddClaim(new Claim(SecurityConstants.Claims.PermissionClaimType, permission));
                }
            }
            if (!user.Roles.IsNullOrEmpty())
            {
                foreach (var role in user.Roles)
                {
                    result.AddClaim(new Claim(ClaimTypes.Role, role.Id));
                }
            }
        
            return result;
        }
      
    }
}
