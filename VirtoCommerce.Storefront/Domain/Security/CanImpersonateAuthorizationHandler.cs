using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CanImpersonateAuthorizationRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "CanImpersonate";
    }
    public class CanImpersonateAuthorizationHandler : AuthorizationHandler<CanImpersonateAuthorizationRequirement>
    {
        private readonly IStoreModule _storeApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        public CanImpersonateAuthorizationHandler(IStoreModule storeApi, IWorkContextAccessor workContextAccessor)
        {
            _storeApi = storeApi;
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanImpersonateAuthorizationRequirement requirement)
        {
            var workContext = _workContextAccessor.WorkContext;
            // Validate the requirement against the resource and identity.

            var info = await _storeApi.GetLoginOnBehalfInfoAsync(workContext.CurrentStore.Id, workContext.CurrentUser.Id);
            if (info.CanLoginOnBehalf == true)
            {
                context.Succeed(requirement);
            }
        }
    }
}
