using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class IsUserSuspendedSpecification: ISpecification<User>
    {
        public bool IsSatisfiedBy(User obj)
        {
            return obj.UserState == AccountState.Rejected;
        }
    }
}
