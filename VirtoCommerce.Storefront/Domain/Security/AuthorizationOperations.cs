using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class AuthorizationOperations
    {
        public static OperationAuthorizationRequirement CanImpersonate { get; } = new OperationAuthorizationRequirement { Name = "CanImpersonate" };
        public static OperationAuthorizationRequirement CanResetCache { get; } = new OperationAuthorizationRequirement { Name = "CanResetCache" };
    }
}
