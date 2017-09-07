namespace VirtoCommerce.Storefront.Common
{
    public class HmacCredentials
    {
        public string AppId { get; set; }
        public string SecretKey { get; set; }

        public HmacCredentials(string appId, string secretKey)
        {
            AppId = appId;
            SecretKey = secretKey;
        }
    }
}
