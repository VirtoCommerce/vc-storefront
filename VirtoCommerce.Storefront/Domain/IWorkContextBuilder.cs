using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IWorkContextBuilder
    {
        HttpContext HttpContext { get; }
        WorkContext WorkContext { get; }
    }
}
