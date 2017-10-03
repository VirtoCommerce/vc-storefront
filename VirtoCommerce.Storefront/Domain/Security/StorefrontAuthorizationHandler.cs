using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class StorefrontAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IStoreModule _storeApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public StorefrontAuthorizationHandler(IStoreModule storeApi, IWorkContextAccessor workContextAccessor)
        {
            _storeApi = storeApi;
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            // Validate the requirement against the resource and identity.
            if(requirement == AuthorizationOperations.CanImpersonate)
            {
                var workContext = _workContextAccessor.WorkContext;
                var info = await _storeApi.GetLoginOnBehalfInfoAsync(workContext.CurrentStore.Id, workContext.CurrentUser.Id);
                if (info.CanLoginOnBehalf == true)
                {
                    context.Succeed(requirement);
                }           
            }            
        }
    }
}
