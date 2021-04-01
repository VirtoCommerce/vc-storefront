using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Feedback
{
    public interface IFeedbackItemService<T, U>
    {
        Task<U> SendAsync(T item);
    }
}
