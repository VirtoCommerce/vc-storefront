namespace VirtoCommerce.Storefront.Domain
{
    public class FormFileStorageOptions
    {
        public const string SectionName = "VirtoCommerce:FormFileStorage";

        public string BlobFolderUrl { get; set; } = "storefront/blob";

        public long FileSizeLimit { get; set; } = 20971520L;

        public string[] PermittedExtensions { get; set; } = { ".jpeg", ".jpg", ".png", ".pdf" };
    }
}
