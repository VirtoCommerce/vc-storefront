using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class IsUserLockedByRequiredEmailVerificationSpecification : ISpecification<User>
    {
        public virtual bool IsSatisfiedBy(User user)
        {
            return user.Status == "Locked" && !user.EmailConfirmed;
        }
    }
}
