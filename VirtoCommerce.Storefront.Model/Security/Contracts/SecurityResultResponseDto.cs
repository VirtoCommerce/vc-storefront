using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Security.Contracts
{
    public class SecurityResultResponseDto
    {
        public SecurityResultDto SecurityResult { get; set; }
    }

    public class SecurityResultDto
    {
        public bool? Succeeded { get; set; }
        public IList<ErrorDto> Errors { get; set; }
    }

    public class ErrorDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
