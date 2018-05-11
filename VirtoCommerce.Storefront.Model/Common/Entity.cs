using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Model.Common
{
    public abstract class Entity : ValueObject, IEntity
    {
        public string Id { get; set; }

        public bool IsTransient()
        {
            return Id == null;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }

    }
}
