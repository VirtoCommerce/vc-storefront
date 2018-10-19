namespace VirtoCommerce.Storefront.Model.Common.Specifications
{
    public interface ISpecification<in T>
    {
        bool IsSatisfiedBy(T obj);
    }
}
