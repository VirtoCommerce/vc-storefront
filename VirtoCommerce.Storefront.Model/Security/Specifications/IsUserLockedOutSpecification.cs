using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class IsUserLockedOutSpecification : ISpecification<User>
    {
        public virtual bool IsSatisfiedBy(User obj)
        {
            return obj.IsLockedOut;
        }
    }
}
