using System;
using System.ComponentModel.DataAnnotations;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class OrganizationUserRegistration : UserRegistration
    {
        [Required]
        public string Role
        {
            get => User.Role.Name;
            set => User.Roles = new[] { new Role { Id = value, Name = value } };
        }

        [Required]
        public string OrganizationId
        {
            get => Contact.OrganizationId;
            set => Contact.OrganizationId = value;
        }
    }
}
