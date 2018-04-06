using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Customer
{
    /// <summary>
    /// Represent customer information structure 
    /// </summary>
    public partial class Contact : Member
    {
        public bool IsActive { get; set; } = true;
        public string FullName { get; set; }
        /// <summary>
        /// Returns the first name of the customer.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Returns the last name of the customer.
        /// </summary>
        public string LastName { get; set; }

        public string MiddleName { get; set; }


        public string TimeZone { get; set; }
        public string DefaultLanguage { get; set; }

        public Address DefaultBillingAddress { get; set; }
        public Address DefaultShippingAddress { get; set; }

        public string OrganizationId { get; set; }
        public Organization Organization { get; set; }

        /// <summary>
        /// Returns true if the customer accepts marketing, returns false if the customer does not.
        /// </summary>
        public bool AcceptsMarketing { get; set; }

        /// <summary>
        /// Returns the default customer_address.
        /// </summary>
        public Address DefaultAddress { get; set; }

        /// <summary>
        /// All contact security accounts
        /// </summary>
        public IEnumerable<SecurityAccount> SecurityAccounts { get; set; }   

    }
}
