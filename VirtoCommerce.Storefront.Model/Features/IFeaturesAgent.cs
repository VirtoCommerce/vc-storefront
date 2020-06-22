namespace VirtoCommerce.Storefront.Model.Features
{
    using Newtonsoft.Json.Linq;

    public interface IFeaturesAgent
    {
        bool IsActive(string featureName, JObject jObject);
    }
}
