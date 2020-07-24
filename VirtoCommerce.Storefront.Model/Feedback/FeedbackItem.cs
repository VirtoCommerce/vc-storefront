using System.Net;

namespace VirtoCommerce.Storefront.Model.Feedback
{
    public class FeedbackItem
    {
        public FeedbackItem(string url) => Url = url;

        public string Url { get; }

        public void SendRequest(params string[] parameters)
        {
            var request = WebRequest.Create($"{Url}&{string.Join('&', parameters)}");
            var response = request.GetResponse();
            response.Dispose();
        }
    }
}
