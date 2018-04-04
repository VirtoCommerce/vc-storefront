using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UsersInvitation : ValueObject
    {
        public string Message { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Emails { get; set; }
    }
}
