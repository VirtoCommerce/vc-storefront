using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class NewPaymentData
    {
        public PaymentIn Payment { get; set; }
        public IEnumerable<PaymentMethod> PaymentMethods { get; set; }
        public CustomerOrder Order { get; set; }
    }
}
