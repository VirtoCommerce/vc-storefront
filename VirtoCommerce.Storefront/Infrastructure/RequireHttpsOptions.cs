namespace VirtoCommerce.Storefront.Infrastructure
{
    public class RequireHttpsOptions
    {
        public bool Enabled { get; set; } = false;
        public int StatusCode { get; set; } = 308;
        public string ReasonPhrase { get; set; } = "Permanent Redirect";
        public int Port { get; set; } = 443;
    }
}
