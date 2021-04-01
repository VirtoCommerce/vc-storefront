using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IFeedbackItemFactory
    {
        FeedbackItem GetItem(string name);
    }
}
