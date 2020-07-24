using VirtoCommerce.Storefront.Model.Feedback;

namespace VirtoCommerce.Storefront.Domain
{
    public interface IFeedbackItemFactory
    {
        void CreateItem(string name);
        FeedbackItem this[string name] { get; }
    }
}
