using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Authentication
{
    public class CustomUserManager : AspNetUserManager<CustomerInfo>
    {
        private readonly IStorefrontSecurity _commerceCoreApi;
        public CustomUserManager(IStorefrontSecurity commerceCoreApi, IUserStore<CustomerInfo> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<CustomerInfo> passwordHasher,
                                IEnumerable<IUserValidator<CustomerInfo>> userValidators, IEnumerable<IPasswordValidator<CustomerInfo>> passwordValidators,
                                ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<CustomerInfo>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _commerceCoreApi = commerceCoreApi;
        }

        public async override Task<bool> CheckPasswordAsync(CustomerInfo user, string password)
        {
            var result = await _commerceCoreApi.PasswordSignInAsync(user.UserName, password);

            return result.Status.EqualsInvariant("success");          
        }
       
    }
}
