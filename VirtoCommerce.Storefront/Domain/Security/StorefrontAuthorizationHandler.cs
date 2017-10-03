using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class StorefrontAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IStoreModule _storeApi;
        private readonly ISecurity _platformSecurityApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public StorefrontAuthorizationHandler(IStoreModule storeApi, ISecurity platformSecurityApi, IWorkContextAccessor workContextAccessor)
        {
            _storeApi = storeApi;
            _platformSecurityApi = platformSecurityApi;
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            var workContext = _workContextAccessor.WorkContext;
            // Validate the requirement against the resource and identity.
            if (requirement == AuthorizationOperations.CanImpersonate)
            {             
                var info = await _storeApi.GetLoginOnBehalfInfoAsync(workContext.CurrentStore.Id, workContext.CurrentUser.Id);
                if (info.CanLoginOnBehalf == true)
                {
                    context.Succeed(requirement);
                }           
            }    
            else if(requirement == AuthorizationOperations.CanResetCache)
            {
                var result = await _platformSecurityApi.UserHasAnyPermissionAsync(workContext.CurrentUser.UserName, new List<string> { "cache:reset" }, new List<string>());
                if (result.Result ?? false)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
