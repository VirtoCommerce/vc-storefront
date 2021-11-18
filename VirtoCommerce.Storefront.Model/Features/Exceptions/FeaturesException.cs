using System;
using System.Runtime.Serialization;

namespace VirtoCommerce.Storefront.Model.Features.Exceptions
{
    [Serializable]
    public class FeaturesException : Exception
    {
        protected FeaturesException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

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
