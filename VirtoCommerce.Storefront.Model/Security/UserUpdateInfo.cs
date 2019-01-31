using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class UserUpdateInfo : Entity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Budget { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public IList<string> Roles { get; set; }
    }
}
