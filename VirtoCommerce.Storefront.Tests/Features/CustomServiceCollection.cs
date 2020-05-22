namespace VirtoCommerce.Storefront.Tests.Features
{
    using Microsoft.Extensions.DependencyInjection;

    using VirtoCommerce.Storefront.Model.Features;

    public class CustomServiceCollection : ServiceCollection
    {
        public CustomServiceCollection()
        {
            this.AddSingleton<IFeaturesAgent, FeaturesAgent>();
        }
    }
}
