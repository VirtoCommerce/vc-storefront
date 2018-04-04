using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserInvitation : ValueObject
    {
        public string OrganizationId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public IList<string> Roles { get; set; }
    }
}
