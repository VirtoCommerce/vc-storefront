using System;
using System.Runtime.Serialization;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Infrastructure.Exceptions
{
    [Serializable]
    public class NoStoresException : StorefrontException
    {
        [Obsolete(DiagnosticId = "SYSLIB0051")]
        protected NoStoresException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NoStoresException() : base("No stores defined", "NoStore")
        {
        }
    }
}
