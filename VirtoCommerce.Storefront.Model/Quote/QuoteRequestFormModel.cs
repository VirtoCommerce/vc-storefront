using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Quote
{
    public partial class QuoteRequestFormModel : Entity
    {
        public QuoteRequestFormModel()
        {
            Items = new List<QuoteItemFormModel>();
        }

        public string Tag { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }

        public Address BillingAddress { get; set; }

        public Address ShippingAddress { get; set; }

        public IList<QuoteItemFormModel> Items { get; set; }
    }
}