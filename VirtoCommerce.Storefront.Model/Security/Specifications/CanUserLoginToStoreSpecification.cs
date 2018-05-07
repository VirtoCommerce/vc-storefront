using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Specifications;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model.Security.Specifications
{
    public class CanUserLoginToStoreSpecification : ISpecification<Store>
    {
        private readonly User _user;
        public CanUserLoginToStoreSpecification(User user)
        {
            _user = user;
        }
        public virtual bool IsSatisfiedBy(Store store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }
            //Allow to login to store for administrators or for users not assigned to store
            var result = _user.IsAdministrator || _user.StoreId.IsNullOrEmpty();
            if(!result)
            {   
                result = store.TrustedGroups.Concat(new[] { store.Id } ).Contains(_user.StoreId);
            }
            return result;
        }

    }
}
