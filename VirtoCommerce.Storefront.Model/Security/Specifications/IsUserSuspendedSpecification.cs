using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class IsUserSuspendedSpecification: ISpecification<User>
    {
        public bool IsSatisfiedBy(User user)
        {
            return user.UserState == AccountState.Rejected;
        }
    }
}
