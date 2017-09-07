using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model
{
    public partial class ContactForm
    {
        public ContactForm()
        {
            Contact = new Dictionary<string, string[]>();
        }

        public IDictionary<string, string[]> Contact { get; set; }

        public string FormType { get; set; }
    }
}
