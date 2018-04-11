using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        public string Permission { get; set; }
    }
}
