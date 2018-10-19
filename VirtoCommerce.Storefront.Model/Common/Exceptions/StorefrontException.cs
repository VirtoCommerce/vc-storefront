using System;

namespace VirtoCommerce.Storefront.Model.Common.Exceptions
{
    public class StorefrontException : Exception
    {
        public string View { get; set; }

        public StorefrontException(string message)
           : this(message, null)
        {
        }
        public StorefrontException(string message, string view)
            : this(message, null, view)
        {
        }

        public StorefrontException(string message, Exception innerException, string view)
            : base(message, innerException)
        {
            View = view;
        }
    }
}
