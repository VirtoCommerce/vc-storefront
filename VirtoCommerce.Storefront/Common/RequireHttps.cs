namespace VirtoCommerce.Storefront.Common
{
    public class RequireHttps
    {
        public bool Enabled { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public int Port { get; set; }
    }
}
