using System;

namespace VirtoCommerce.Storefront.Model.Common.Caching
{
    public sealed class TokenCancelledEventArgs : EventArgs
    {
        public TokenCancelledEventArgs(string tokenKey)
        {
            TokenKey = tokenKey;
        }

        public string TokenKey { get; }
        public override string ToString()
        {
            return $"{TokenKey}";
        }
    }
}
