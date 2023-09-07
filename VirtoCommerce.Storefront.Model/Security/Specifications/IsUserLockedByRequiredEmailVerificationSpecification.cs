using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Specifications;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class IsUserLockedByRequiredEmailVerificationSpecification : ISpecification<Store>
    {
        private readonly User _user;

        public IsUserLockedByRequiredEmailVerificationSpecification(User user)
        {
            _user = user;
        }

        public virtual bool IsSatisfiedBy(Store store)
        {
            return _user.Contact.Status == "Locked"
                && !_user.EmailConfirmed
                && store.Settings.GetSettingValue<bool>("Stores.EmailVerificationEnabled", false)
                && store.Settings.GetSettingValue<bool>("Stores.EmailVerificationRequired", false);
        }
    }
}
