using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Subscriptions;

namespace VirtoCommerce.Storefront.Model.Security
{
    public partial class User : Entity
    {
        public User()
        {
            ExternalLogins = new List<ExternalUserLoginInfo>();
        }

        /// <summary>
        /// Store id
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// Security account user name
        /// </summary>
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        /// <summary>
        /// Returns the email address of the customer.
        /// </summary>
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string DefaultLanguage { get; set; }

        public bool TwoFactorEnabled { get; set; }
        public bool IsLockedOut
        {
            get
            {
                return LockoutEndDateUtc != null ? LockoutEndDateUtc.Value > DateTime.UtcNow : false;
            }
        }

        /// <summary>
        ///  Used to record failures for the purposes of lockout
        /// </summary>
        public virtual int AccessFailedCount { get; set; }
        /// <summary>
        /// Is lockout enabled for this user
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }
        /// <summary>
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        /// <summary>
        /// Returns true if user authenticated  returns false if it anonymous. 
        /// </summary>
        public bool IsRegisteredUser { get; set; }
        /// <summary>
        /// The flag indicates that the user is an administrator 
        /// </summary>
        public bool IsAdministrator { get; set; }
        public string UserType { get; set; }
        public AccountState UserState { get; set; }
        /// <summary>
        /// The user ID of an operator who has loggen in on behalf of a customer
        /// </summary>
        public string OperatorUserId { get; set; }
        /// <summary>
        /// The user name of an operator who has loggen in on behalf of a customer
        /// </summary>
        public string OperatorUserName { get; set; }

        public IList<ExternalUserLoginInfo> ExternalLogins { get; set; }

        //Selected and persisted currency code
        public string SelectedCurrencyCode { get; set; }

        public string ContactId { get; set; }
        /// <summary>
        /// Member associated with user 
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// All user permissions
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }

        /// <summary>
        /// Single user role
        /// </summary>
        public Role Role
        {
            get
            {
                return Roles?.FirstOrDefault();
            }
        }
        /// <summary>
        /// All user roles
        /// </summary>
        public IEnumerable<Role> Roles { get; set; }

        /// <summary>
        /// All user orders
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IMutablePagedList<CustomerOrder> Orders { get; set; }

        /// <summary>
        /// All user RFQ
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IMutablePagedList<QuoteRequest> QuoteRequests { get; set; }

        /// <summary>
        /// All user subscriptions
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public IMutablePagedList<Subscription> Subscriptions { get; set; }
    }
}
