using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class ContactForm : ValueObject
    {
        public ContactForm()
        {
            Contact = new Dictionary<string, string[]>();
        }

        public IDictionary<string, string[]> Contact { get; set; }

        public string FormType { get; set; }
    }
}
