using VirtoCommerce.LiquidThemeEngine.Objects;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class AttachmentConverter
    {
        public static Attachment ToShopifyModel(this VirtoCommerce.Storefront.Model.Attachment attachment)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidAttachment(attachment);
        }

    }

    public partial class ShopifyModelConverter
    {
        public virtual Attachment ToLiquidAttachment(VirtoCommerce.Storefront.Model.Attachment attachment)
        {
            var retVal = new Attachment();

            retVal.CreatedBy = attachment.CreatedBy;
            retVal.CreatedDate = attachment.CreatedDate;
            retVal.MimeType = attachment.MimeType;
            retVal.ModifiedBy = attachment.ModifiedBy;
            retVal.ModifiedDate = attachment.ModifiedDate;
            retVal.Name = attachment.Name;
            retVal.Size = attachment.Size;
            retVal.Url = attachment.Url;         

            return retVal;
        }
    }
}