namespace VirtoCommerce.Storefront.Model.Common
{
    public interface IConvertible<out T>
    {
        T ConvertTo(Currency currency);
    }
}
