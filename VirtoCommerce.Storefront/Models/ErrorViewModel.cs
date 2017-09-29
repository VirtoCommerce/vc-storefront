using System;

namespace VirtoCommerce.Storefront.Models
{
    public class ErrorViewModel
    {
        public string Code { get; set; }
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}