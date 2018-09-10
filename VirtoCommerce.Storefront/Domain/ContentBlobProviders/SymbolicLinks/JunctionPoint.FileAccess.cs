using System;

namespace VirtoCommerce.Storefront.Domain.ContentBlobProviders.SymbolicLinks
{
    public static partial class JunctionPoint
    {
        [Flags]
        private enum FileAccess : uint
        {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000,
        }
    }
}
