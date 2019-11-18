namespace VirtoCommerce.Storefront.Tests.Scriban
{
    public class MyFunctions
    {
        public static string T(object input, params object[] variables)
        {
            return string.Format(input.ToString(), variables);
        }

    }
}
