using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain
{
    public class WorkContextAccessor : IWorkContextAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public WorkContext WorkContext
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Items["WorkContext"] as WorkContext;
            }
            set
            {
                _httpContextAccessor.HttpContext.Items["WorkContext"] = value;
            }
        }

    }
}
