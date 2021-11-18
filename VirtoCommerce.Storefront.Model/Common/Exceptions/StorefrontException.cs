using System;
using System.Runtime.Serialization;

namespace VirtoCommerce.Storefront.Model.Common.Exceptions
{
    [Serializable]
    public class StorefrontException : Exception
    {
        public string View { get; set; }

        protected StorefrontException(SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            View = info.GetString("View");
        }

        public StorefrontException()
        {

        }

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

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("View", View);
        }
    }
}
