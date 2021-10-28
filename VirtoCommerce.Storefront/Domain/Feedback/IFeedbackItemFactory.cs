using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Domain.Feedback
{
    public interface IFeedbackItemFactory
    {
        FeedbackItem GetItem(string name);
    }
}
