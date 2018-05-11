using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IWorkContextBuilder
    {
        HttpContext HttpContext { get; }
        WorkContext WorkContext { get; }
    }
}
