using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Domain.Feedback
{
    public class FeedbackItemFactory : IFeedbackItemFactory
    {
        public FeedbackItemFactory(IConfigurationSection section) => Config(section);

        private readonly Dictionary<string, FeedbackItem> _items = new Dictionary<string, FeedbackItem>();

        public FeedbackItem GetItem(string name) => _items[name].Clone();

        public void Config(IConfigurationSection configuration)
        {
            var services = configuration.GetChildren();
            foreach (var service in services)
            {
                var url = service.GetSection("Url");
                if (url == null)
                {
                    throw new KeyNotFoundException("Url segment not found in config object with specified key.");
                }

                bool.TryParse(service.GetSection("AllowAdditionalParams").Value, out var allowAdditionalParams);
                var item = new FeedbackItem(url.Value)
                {
                    HttpMethod = service.GetSection("Method").Value,
                    AllowAdditionalParams = allowAdditionalParams
                };

                var parameters = service.GetSection("Params");
                if (parameters != null)
                {
                    item.Parameters = parameters.GetChildren()
                        .ToList()
                        .Select(p => $"{p.GetValue<string>("Name")}={p.GetValue<string>("Value")}")
                        .ToList();
                }
                _items.Add(service.Key, item);
            }
        }
    }
}
