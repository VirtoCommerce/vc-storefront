namespace VirtoCommerce.Storefront.Model.Features.Exceptions
{
    using System;

    public class FeaturesException : Exception
    {
        public FeaturesException(string message)
            : base(message)
        {
        }

        public FeaturesException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
