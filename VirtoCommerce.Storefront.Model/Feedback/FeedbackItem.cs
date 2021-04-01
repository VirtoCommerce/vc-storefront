using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.Storefront.Model.Feedback
{
    public class FeedbackItem
    {
        public FeedbackItem(string url) => Url = url;

        public string Url { get; }

        public string HttpMethod { get; set; }

        public bool AllowAdditionalParams { get; set; }

        public List<string> AdditionalParams { get; set; } = new List<string>();

        private List<string> _parameters = new List<string>();

        public List<string> Parameters
        {
            get => AllowAdditionalParams ? _parameters.Concat(AdditionalParams).ToList() : _parameters;
            set => _parameters = value;
        }

        public FeedbackItem Clone()
        {
            return new FeedbackItem(Url)
            {
                HttpMethod = HttpMethod,
                AllowAdditionalParams = AllowAdditionalParams,
                AdditionalParams = new List<string>(AdditionalParams),
                Parameters = new List<string>(_parameters)
            };
        }
    }
}
