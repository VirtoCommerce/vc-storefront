using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Order.Events
{
    /// <summary>
    /// Event generated when user logged in to the storefront
    /// </summary>
    public partial class UserLoginEvent : DomainEvent
    {
        public UserLoginEvent(WorkContext workContext, CustomerInfo user)
        {
            WorkContext = workContext;
            User = user;
        }

        public WorkContext WorkContext { get; set; }
        public CustomerInfo User { get; set; }
    }
}
