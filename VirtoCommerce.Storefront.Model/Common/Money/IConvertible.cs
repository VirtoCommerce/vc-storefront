namespace VirtoCommerce.Storefront.Model.Common
{
    public interface IConvertible<T>
    {
        T ConvertTo(Currency currency);
    }
}
