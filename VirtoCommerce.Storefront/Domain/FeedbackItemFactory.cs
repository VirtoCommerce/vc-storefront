using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Domain
{
    public class FeedbackItemFactory : IFeedbackItemFactory
    {
        private readonly IConfiguration _configuration;

        public FeedbackItemFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private Dictionary<string, FeedbackItem> _items = new Dictionary<string, FeedbackItem>();

        public FeedbackItem this[string name]
        {
            get => _items[name];
        }

        public void CreateItem(string name)
        {
            var url = _configuration.GetSection($"AzureLogicApps:{name}:Url").Value;
            if (url != null)
            {
                _items.Add(name, new FeedbackItem(url));
            }
            else
            {
                throw new KeyNotFoundException("Url segment not found in config object with specified key.");
            }
        }
    }
}
