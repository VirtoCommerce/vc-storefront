using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using StorefrontModel = VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class ContactUsFormConverter
    {
        public static Form ToShopifyModel(this StorefrontModel.ContactForm contactUsForm)
        {
            var retVal = new Form();
            if (contactUsForm.Contact != null)
            {
                retVal.Properties = contactUsForm.Contact.ToDictionary(x => x.Key, x => x.Value != null ? string.Join(", ", x.Value) : string.Empty);
            }

            return retVal;
        }
    }
}