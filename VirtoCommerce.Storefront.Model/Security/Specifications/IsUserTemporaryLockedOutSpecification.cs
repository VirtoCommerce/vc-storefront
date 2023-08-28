using System;
using VirtoCommerce.Storefront.Model.Common.Specifications;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class IsUserTemporaryLockedOutSpecification : ISpecification<User>
    {
        public virtual bool IsSatisfiedBy(User user)
        {
            return user.LockoutEndDateUtc != DateTime.MaxValue.ToUniversalTime();
        }
    }
}
